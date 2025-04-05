from django.core.cache import cache
from rest_framework import status
from rest_framework.decorators import api_view, permission_classes
from rest_framework.parsers import MultiPartParser, FormParser
from rest_framework.response import Response
from rest_framework.views import APIView

from core.guards.permission_classes import IsOrganizer
from common.utils.cache_utils import delete_cache
from events.api.serializer import EventSerializerOrganizer
from events.services.event_service import EventService

# Create your views here.
class EventOrganizerAPIVIEW(APIView):
    permission_classes = [IsOrganizer]
    service = EventService()
    parser_classes = (MultiPartParser, FormParser)
    cache_key = 'events'

    def get(self, request):
        if cache.get(self.cache_key):
            return Response(cache.get(self.cache_key), status=status.HTTP_200_OK)

        events = self.service.get_all()
        if not events:
            return Response({'message': "There are not events in database"} ,status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializerOrganizer(events, many=True)
        cache.set(self.cache_key, serializer.data)
        return Response(serializer.data, status=status.HTTP_200_OK)

    def post(self, request):
        delete_cache(self.cache_key)

        serializer = EventSerializerOrganizer(data=request.data, context={'request': request})
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request):
        delete_cache(self.cache_key)

        event = self.service.get_by_id(request.data['id'])
        if event is None or event.organizer != request.user:
            return Response({'message': 'Event not found or you do not have permission to edit this event'},
                            status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializerOrganizer(event, data=request.data, context={'request': request})
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_200_OK)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request):
        delete_cache(self.cache_key)

        event = self.service.get_by_id(request.data['id'])
        if event is None or event.organizer != request.user:
            return Response({'message': 'Event not found or you do not have permission to delete this event'},
                            status=status.HTTP_404_NOT_FOUND)

        event.delete()
        return Response({'message': 'Event deleted'}, status=status.HTTP_200_OK)

@api_view(['PUT'])
@permission_classes([IsOrganizer])
def close_event(request):
    from core.reports.generator import generate_close_event_report
    from events.models import Event

    if not request.data['id']:
        return Response({'message': 'Event id is required'}, status=status.HTTP_400_BAD_REQUEST)

    try:
        event = Event.objects.get(title=request.data['id'])
        if not event.active or event.deleted_at is not None:
            return Response({'message': 'Event not found'}, status=status.HTTP_404_NOT_FOUND)
    except Event.DoesNotExist:
        return Response({'message': 'Event not found'}, status=status.HTTP_404_NOT_FOUND)

    if event is None or event.organizer != request.user:
        return Response({'message': 'Event not found or you do not have permission to close this event'},
                        status=status.HTTP_404_NOT_FOUND)

    # Generate event report.
    generate_close_event_report(event)

    event.delete()
    event.save()
    return Response({'message': 'Event closed and report generated'}, status=status.HTTP_200_OK)



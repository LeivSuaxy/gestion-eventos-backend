from rest_framework import status
from rest_framework.response import Response
from rest_framework.views import APIView

from common.strategy.authpermission import IsOrganizer
from events.api.serializer import EventSerializerOrganizer
from events.services.eventservice import EventService

from rest_framework.parsers import MultiPartParser, FormParser
from django.core.cache import cache
from common.utils.cache_utils import delete_cache

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
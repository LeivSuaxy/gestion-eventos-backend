from rest_framework import status
from rest_framework.response import Response
from rest_framework.views import APIView

from common.strategy.authpermission import IsOrganizer
from events.api.serializer import EventSerializerOrganizer
from events.services.eventservice import EventService

from rest_framework.parsers import MultiPartParser, FormParser
from django.core.cache import cache

# Create your views here.
class EventOrganizerAPIVIEW(APIView):
    permission_classes = [IsOrganizer]
    service = EventService()
    parser_classes = (MultiPartParser, FormParser)

    def get(self, request):
        if cache.get('events'):
            return Response(cache.get('events'), status=status.HTTP_200_OK)

        events = self.service.get_all()
        if not events:
            return Response({'message': "There are not events in database"} ,status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializerOrganizer(events, many=True)
        cache.set('events', serializer.data)
        return Response(serializer.data, status=status.HTTP_200_OK)

    def post(self, request):
        if cache.get('events'):
            cache.delete('events')

        serializer = EventSerializerOrganizer(data=request.data, context={'request': request})
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)
from rest_framework import status
from rest_framework.response import Response
from rest_framework.views import APIView

from common.strategy.authpermission import IsOrganizer
from events.api.serializer import EventSerializerOrganizer
from events.services.eventservice import EventService


# Create your views here.
class EventOrganizerAPIVIEW(APIView):
    permission_classes = [IsOrganizer]
    service = EventService()

    def get(self, request):
        events = self.service.get_all()
        if not events:
            return Response({'message': "There are not events in database"} ,status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializerOrganizer(events, many=True)
        return Response(serializer.data, status=status.HTTP_200_OK)
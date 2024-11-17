from events.models import Event
from events.api.serializer import EventSerializer
from rest_framework.response import Response
from rest_framework import status

class AdminEventService:
    @staticmethod
    def get():
        events = Event.objects.all()

        if not events:
            return Response({'message': 'Not found events'}, status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializer(events, many=True)

        if serializer.is_valid():
            return Response(serializer.data, status=status.HTTP_200_OK)

        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)



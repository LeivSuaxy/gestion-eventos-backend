from events.models import Event
from events.api.serializer import EventSerializer
from rest_framework.response import Response
from rest_framework import status
from datetime import datetime

class AdminEventService:
    @staticmethod
    def get():
        events = Event.objects.all()

        if not events:
            return Response({'message': 'Not found events'}, status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializer(events, many=True)
        return Response(serializer.data, status=status.HTTP_200_OK)

    @staticmethod
    def post(data):
        title = data.get('title')
        description = data.get('description')
        # date = data.get('date')
        date = datetime.now()
        price = data.get('price')
        location = data.get('location')

        if not title or not description or not date or not price or not location:
            return Response({'message': 'Missing required fields'}, status=status.HTTP_400_BAD_REQUEST)

        event = Event.objects.create(title=title, description=description, date=date, price=price, location=location)
        event.save()

        return Response({'message': 'Event created'}, status=status.HTTP_201_CREATED)

    @staticmethod
    def put(data):
        pass

    @staticmethod
    def delete(data):
        pass



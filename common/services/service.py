from events.models import Event, ProcessedEvent
from events.api.serializer import EventSerializer, ProcessedEventSerializer
from rest_framework.response import Response
from rest_framework import status
from datetime import datetime
from common.validators.params_validator import params_validator

# Common for administration and organizers
class EventService:
    @staticmethod
    def get():
        events = Event.objects.all()

        if not events:
            return Response({'message': 'Not found events'}, status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializer(events, many=True)
        return Response(serializer.data, status=status.HTTP_200_OK)

    @staticmethod
    def post(data):
        validated, info = params_validator(data, ['title', 'description', 'price', 'location'])

        if not validated:
            return Response({'error': info}, status=status.HTTP_400_BAD_REQUEST)

        title = data.get('title')
        description = data.get('description')
        # date = data.get('date')
        date = datetime.now()
        price = data.get('price')
        location = data.get('location')

        event = Event.objects.create(title=title, description=description, date=date, price=price, location=location)
        event.save()

        return Response({'message': 'Event created'}, status=status.HTTP_201_CREATED)

    @staticmethod
    def put(data):
        validate, info = params_validator(data, ['id'])

        if not validate:
            return Response({'erro': info}, status=status.HTTP_400_BAD_REQUEST)

        event_id = data.get('id')

        try:
            event = Event.objects.get(id=event_id)
        except Event.DoesNotExist:
            return Response({'message': 'Event not found'}, status=status.HTTP_404_NOT_FOUND)

        title = data.get('title')
        description = data.get('description')
        # date = data.get('date')
        price = data.get('price')
        location = data.get('location')

        if title:
            event.title = title
        if description:
            event.description = description
        # if date:
        # event.date = date
        if price:
            event.price = price
        if location:
            event.location = location

        event.save()

        return Response({'message': 'Event updated'}, status=status.HTTP_200_OK)

    @staticmethod
    def delete(data):
        event_id = data.get('id')

        if not event_id:
            return Response({'message': 'ID is required'}, status=status.HTTP_400_BAD_REQUEST)

        try:
            event = Event.objects.get(id=event_id)
        except Event.DoesNotExist:
            return Response({'message': 'Event not found'}, status=status.HTTP_404_NOT_FOUND)

        event.delete()

        return Response({'message': 'Event deleted'}, status=status.HTTP_200_OK)

class EventProcessedService:
    @staticmethod
    def get():
        events = ProcessedEvent.objects.all()

        if not events:
            return Response({'message': 'Not found events'}, status=status.HTTP_404_NOT_FOUND)

        serializer = ProcessedEventSerializer(events, many=True)
        return Response(serializer.data, status=status.HTTP_200_OK)

    @staticmethod
    def delete(data):
        event_id = data.get('id')

        if not event_id:
            return Response({'message': 'ID is required'}, status=status.HTTP_400_BAD_REQUEST)

        try:
            event = ProcessedEvent.objects.get(id=event_id)
        except ProcessedEvent.DoesNotExist:
            return Response({'message': 'Event not found'}, status=status.HTTP_404_NOT_FOUND)

        event.delete()

        return Response({'message': 'Event deleted'}, status=status.HTTP_200_OK)


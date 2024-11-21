from events.models import Event, EventParticipant, ProcessedEvent
from datetime import datetime
from rest_framework.response import Response
from rest_framework import status

def process_events(file_id: str):
    events = Event.objects.filter(date__lt=datetime.now())

    if not events:
        return Response({'message': 'No events found'}, status=status.HTTP_404_NOT_FOUND)

    for event in events:
        processed_event = ProcessedEvent.objects.create(
            old_id=event.id,
            title=event.title,
            description=event.description,
            date=event.date,
            price=event.price,
            location=event.location,
            participant_quantity=EventParticipant.objects.filter(event=event).count(),
            report_id=file_id
        )
        processed_event.save()
        event.delete()

    return Response({'message': 'Events processed'}, status=status.HTTP_200_OK)

def register_at_event(data, user):
    event = data.get('event')

    if not user:
        return Response({'error': 'Please login first'}, status=status.HTTP_400_BAD_REQUEST)

    if not event:
        return Response({'error': 'Please provide an event id'}, status=status.HTTP_400_BAD_REQUEST)

    _event = Event.objects.get(id=event)

    if not _event:
        return Response({'error': 'The event that has this id was not found'}, status=status.HTTP_404_NOT_FOUND)

    EventParticipant.objects.create(event_id=_event.id, user_id=user.id)

    return Response({'success': 'You have registered successfully'}, status=status.HTTP_201_CREATED)

def unregister_at_event(data, user):
    event = data.get('event')

    if not user:
        return Response({'error': 'Please login first'}, status=status.HTTP_400_BAD_REQUEST)

    if not event:
        return Response({'error': 'Please provide an event id'}, status=status.HTTP_400_BAD_REQUEST)

    _event = Event.objects.get(id=event)

    if not _event:
        return Response({'error': 'The event that has this id was not found'}, status=status.HTTP_404_NOT_FOUND)

    _event_participant = EventParticipant.objects.get(event_id=_event.id, user_id=user.id)

    if not _event_participant:
        return Response({'error': 'You are not registered for this event'}, status=status.HTTP_404_NOT_FOUND)

    _event_participant.delete()

    return Response({'success': 'You have successfully canceled your registration'}, status=status.HTTP_200_OK)

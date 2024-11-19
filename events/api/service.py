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




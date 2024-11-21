from rest_framework import serializers
from events.models import Event, ProcessedEvent

class EventSerializer(serializers.ModelSerializer):
    class Meta:
        model = Event
        fields = ['id' ,'title', 'description', 'date', 'price', 'location']

class ProcessedEventSerializer(serializers.ModelSerializer):
    class Meta:
        model = ProcessedEvent
        fields = ['id', 'old_id', 'title', 'description', 'date', 'price', 'location', 'participant_quantity', 'report_id']
from rest_framework import serializers
from events.models import Event

class EventSerializerAdmin(serializers.ModelSerializer):
    class Meta:
        model = Event
        fields = '__all__'
        extra_kwargs = { 'organizer': {'read_only': True} }
from rest_framework import serializers
from events.models import Event

class EventSerializerUser(serializers.ModelSerializer):
    class Meta:
        model = Event
        exclude = ['created_at',
                   'updated_at',
                   'deleted_at',
                   'active',]
from rest_framework import serializers
from events.models import Event

class EventSerializerOrganizer(serializers.ModelSerializer):
    class Meta:
        model = Event
        fields = '__all__'
        extra_kwargs = { 'organizer': {'read_only': True} }

    def create(self, validated_data):
        request = self.context.get('request')
        if request and hasattr(request, 'user'):
            validated_data['organizer'] = request.user
        return super().create(validated_data)
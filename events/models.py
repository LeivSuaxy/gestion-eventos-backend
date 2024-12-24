from django.db import models
from common.models.base import BaseModel
from authentication.models import EventUser

# Create your models here.
class Event(BaseModel):
    title = models.CharField(max_length=255)
    description = models.TextField()
    date = models.DateField()
    image = models.ImageField(upload_to='static/images/events/')
    is_published = models.BooleanField(default=False)
    organizer = models.ForeignKey(EventUser, on_delete=models.CASCADE, related_name='events')

    class Meta(BaseModel.Meta):
        indexes = BaseModel.Meta.indexes + [
            models.Index(fields=['is_published'])
        ]

    def __str__(self):
        return self.title

class EventParticipant(BaseModel):
    event = models.ForeignKey(Event, on_delete=models.CASCADE, related_name='participants')
    participant = models.ForeignKey(EventUser, on_delete=models.CASCADE, related_name='participations')

    class Meta(BaseModel.Meta):
        unique_together = ('event', 'participant')

    def __str__(self):
        return f'{self.event.title} - {self.participant.username}'
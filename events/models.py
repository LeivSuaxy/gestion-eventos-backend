from django.db import models

from categories.models import Category, SubCategory
from common.models.base import BaseModel
from authentication.models import EventUser

# Create your models here.
class Event(BaseModel):
    title = models.CharField(max_length=255)
    description = models.TextField()
    date = models.DateField()
    image = models.ImageField(upload_to='images/events/', null=True, blank=True)
    is_published = models.BooleanField(default=False)
    require_acceptance = models.BooleanField(default=False)
    limit_participants = models.PositiveIntegerField(default=30, null=True, blank=True)
    organizer = models.ForeignKey(EventUser, on_delete=models.CASCADE, related_name='events')
    category = models.ForeignKey(Category, on_delete=models.CASCADE, related_name='events')
    sub_category = models.ForeignKey(SubCategory, on_delete=models.CASCADE, related_name='events')

    class Meta(BaseModel.Meta):
        indexes = BaseModel.Meta.indexes + [
            models.Index(fields=['is_published'])
        ]

    def __str__(self):
        return self.title

class EventParticipant(BaseModel):
    event = models.ForeignKey(Event, on_delete=models.CASCADE, related_name='participants')
    participant = models.ForeignKey(EventUser, on_delete=models.CASCADE, related_name='participations')
    accepted = models.BooleanField(default=True)

    class Meta(BaseModel.Meta):
        unique_together = ('event', 'participant')

    def save(self, *args, **kwargs):
        if self.event.require_acceptance:
            self.accepted = False
        super().save(*args, **kwargs)

    def __str__(self):
        return f'{self.event.title} - {self.participant.username}'
from django.db import models

# Create your models here.
class Event (models.Model):
    id = models.AutoField(primary_key=True)
    title = models.CharField(max_length=100)
    description = models.TextField()
    date = models.DateTimeField()
    price = models.FloatField()
    location = models.CharField(max_length=255)

    def __str__(self):
        return f'{self.title} with price {self.price}'

class EventParticipant (models.Model):
    id = models.AutoField(primary_key=True)
    event = models.ForeignKey(Event, on_delete=models.CASCADE)
    user = models.ForeignKey('authentication.User', on_delete=models.CASCADE)

    def __str__(self):
        return f'{self.user.username} is going to {self.event.title}'
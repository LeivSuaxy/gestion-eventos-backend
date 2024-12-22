from django.db import models
from django.contrib.auth.models import AbstractUser
from authentication.enum.roles import Role

# Create your models here.
class EventUser(AbstractUser):
    role = models.CharField(max_length=20, choices=[(role.value, role.name) for role in Role], default=Role.USER.value)

from django.db import models
from django.contrib.auth.models import AbstractUser
from authentication.enum.roles import Role
from uuid import uuid4

# Create your models here.
class EventUser(AbstractUser):
    id = models.UUIDField(primary_key=True, default=uuid4, editable=False, unique=True)
    email = models.EmailField(unique=True)
    role = models.CharField(max_length=20, choices=[(role.value, role.name) for role in Role], default=Role.USER.value)

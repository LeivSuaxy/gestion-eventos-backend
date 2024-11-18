from django.db import models
from django.contrib.auth.models import AbstractUser, Group, Permission, PermissionsMixin
from authentication.enum.role import Role

class User(AbstractUser, PermissionsMixin):
    id = models.AutoField(primary_key=True)
    email = models.EmailField(unique=True)
    username = models.CharField(max_length=100, unique=True)
    first_name = models.CharField(max_length=100)
    last_name = models.CharField(max_length=100)
    password = models.CharField(max_length=100)
    is_active = models.BooleanField(default=True)
    rol = models.IntegerField(choices=[(role.value, role.name) for role in Role], default=Role.PARTICIPANT.value)
    phone = models.CharField(max_length=50)
    groups = models.ManyToManyField(Group, related_name='custom_user_set')
    user_permissions = models.ManyToManyField(Permission, related_name='custom_user_set_permissions')
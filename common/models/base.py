from django.db import models
from uuid import uuid4

class BaseModel(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid4(), editable=False, unique=True)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    deleted_at = models.DateTimeField(default=None, null=True)
    active = models.BooleanField(default=True)

    class Meta:
        indexes = [
            models.Index(fields=['active']),
            models.Index(fields=['deleted_at']),
        ]
        abstract = True

from django.db import models
from uuid import uuid4
from django.utils import timezone

class BaseModel(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid4, editable=False, unique=True)
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

    def delete(self, using=None, keep_parents=False):
        self.active = False
        self.deleted_at = timezone.now()
        self.save()

    def hard_delete(self, using=None, keep_parents=False):
        super(BaseModel, self).delete(using, keep_parents)

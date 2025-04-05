from django.db import models

from common.models.base import BaseModel

# Create your models here.
class Category(BaseModel):
    name = models.CharField(max_length=255, unique=True)


class SubCategory(BaseModel):
    name = models.CharField(max_length=255, unique=True)
    category = models.ForeignKey(
        Category,
        on_delete=models.CASCADE,
        related_name='subcategories',
        verbose_name='Category'
    )
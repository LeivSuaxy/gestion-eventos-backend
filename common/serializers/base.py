from rest_framework import serializers
from django.db import models
from abc import ABC, abstractmethod

class BaseSerializerAdmin(serializers.ModelSerializer, ABC):
    """
    Abstract class for serializers in django-rest-framework.
    """

    @property
    @abstractmethod
    def model(self) -> models.Model:
        pass

    @property
    @abstractmethod
    def admin_permission(self) -> str:
        pass

    class Meta:
        fields = '__all__'

    @classmethod
    def get_meta_class(cls):
        class Meta:
            model = cls.model
            fields = '__all__'
        return Meta

    def __init_subclass__(cls, **kwargs):
        super().__init_subclass__(**kwargs)
        cls.Meta = cls.get_meta_class()

    def create(self, validated_data):
        request = self.context.get('request')
        if request and hasattr(request, 'user'):
            validated_data[self.admin_permission] = request.user
        return super().create(validated_data)
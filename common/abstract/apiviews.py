from abc import ABC, abstractmethod
from rest_framework.views import APIView
from common.services.base import BaseService
from django.core.cache import cache
from rest_framework.response import Response
from rest_framework import status
from rest_framework.serializers import ModelSerializer
from common.utils.cache_utils import delete_cache

class BaseApiView(APIView, ABC):
    """
                Abstract class for management views for django-rest-framework.

                REQUIRED VARIABLES:
                - permission_classes: List of permission classes required to access the view.
                - service: Instance of the service that handles the business logic.
                - cache_key: Key used to store and retrieve data in the cache.
                - object_name_many: Plural name of the object, used in response messages.
                - object_name_single: Name of the object in singular, used in response messages.
                - serializer_class: Serializer class used to validate and transform data.
            """

    @property
    @abstractmethod
    def service(self) -> BaseService:
        pass

    @property
    @abstractmethod
    def cache_key(self) -> str:
        pass

    @property
    @abstractmethod
    def object_name_many(self) -> str:
        pass

    @property
    @abstractmethod
    def object_name_single(self) -> str:
        pass

    @property
    @abstractmethod
    def serializer_class(self) -> ModelSerializer:
        pass

    def get(self, request):
        if cache.get(self.cache_key):
            return Response(cache.get(self.cache_key), status=status.HTTP_200_OK)

        data = self.service.get_all()
        if not data:
            return Response({'message': f'There are not {self.object_name_many} in database'},
                            status=status.HTTP_404_NOT_FOUND)

        serializer = self.serializer_class(data, many=True)
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_200_OK)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

class BaseAdminApiView(BaseApiView, ABC):
    def post(self, request):
        delete_cache(self.cache_key)

        serializer = self.serializer_class(data=request.data, context={'request': request})
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_200_OK)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request):
        delete_cache(self.cache_key)

        data = self.service.get_by_id(request.data['id'])
        if data is None:
            return Response({'message': f'{self.object_name_single} not found'}, status=status.HTTP_404_NOT_FOUND)

        serializer = self.serializer_class(data, data=request.data, context={'request': request})
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_200_OK)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request):
        delete_cache(self.cache_key)

        data = self.service.get_by_id(request.data['id'])
        if data is None:
            return Response({'message': f'{self.object_name_single} not found'}, status=status.HTTP_404_NOT_FOUND)

        data.delete()
        return Response({'message': f'{self.object_name_single} deleted'}, status=status.HTTP_200_OK)

class BaseUserApiView(BaseApiView, ABC):
    pass
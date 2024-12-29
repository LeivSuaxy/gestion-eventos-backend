from rest_framework import status
from rest_framework.response import Response
from rest_framework.views import APIView
from public.api.serializer import EventSerializerUser
from public.services.publicservice import PublicEventService


# Create your views here.
class EventUserAPIView(APIView):
    def get(self, request):
        result_page = PublicEventService().get_paginated_events(request)
        serializer = EventSerializerUser(result_page, many=True)
        return Response(serializer.data, status=status.HTTP_200_OK)

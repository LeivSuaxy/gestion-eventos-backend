from django.shortcuts import render
from rest_framework.views import APIView
from rest_framework.parsers import MultiPartParser, FormParser
from common.strategy.authpermission import IsAdmin
from events.services.eventservice import EventService
from rest_framework.response import Response
from administration.api.serializer import EventSerializerAdmin
from rest_framework import status

# Create your views here.
class EventAdminAPIVIEW(APIView):
    permission_classes = [IsAdmin]
    service = EventService()
    parser_classes = (MultiPartParser, FormParser)

    def get(self, request):
        events = self.service.get_all()

        if not events:
            return Response({'message': "There are not events in database"} ,status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializerAdmin(events, many=True)
        return Response(serializer.data, status=status.HTTP_200_OK)

    def post(self, request):
        serializer = EventSerializerAdmin(data=request.data, context={'request': request})
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

    def put(self, request):
        event = self.service.get_by_id(request.data['id'])

        if event is None:
            return Response({'message': 'Event not found'}, status=status.HTTP_404_NOT_FOUND)

        serializer = EventSerializerAdmin(event, data=request.data, context={'request': request})
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_200_OK)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

    def delete(self, request):
        event = self.service.get_by_id(request.data['id'])

        if event is None:
            return Response({'message': 'Event not found'}, status=status.HTTP_404_NOT_FOUND)

        event.delete()
        return Response({'message': 'Event deleted'}, status=status.HTTP_200_OK)
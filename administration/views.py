from rest_framework.decorators import api_view
from rest_framework.permissions import IsAuthenticated
from rest_framework.views import APIView
from common.strategy.authpermission import IsAdmin, IsOrganizer, IsParticipant
from common.services.service import EventService, EventProcessedService

# Create your views here.
# These endpoints are for administration purposes only

class AdminEventView(APIView):
    # permission_classes = [IsAdmin, IsOrganizer, IsParticipant]

    @staticmethod
    def get(request):
        return EventService.get()

    @staticmethod
    def post(request):
        return EventService.post(request.data)

    @staticmethod
    def put(request):
        return EventService.put(request.data)

    @staticmethod
    def delete(request):
        return EventService.delete(request.data)

class AdminProcessedEventView(APIView):
    # permission_classes = [IsAdmin, IsOrganizer]

    @staticmethod
    def get(request):
        return EventProcessedService.get()

    @staticmethod
    def delete(request):
        return EventProcessedService.delete(request.data)


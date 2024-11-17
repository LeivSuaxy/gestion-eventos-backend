from rest_framework.views import APIView
from common.strategy.authpermission import IsOrganizer
from common.services.service import EventService

# Create your views here.
class OrganizerEventView(APIView):
    # permission_classes = [IsOrganizer]

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
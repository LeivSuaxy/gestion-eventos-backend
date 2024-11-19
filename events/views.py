from rest_framework.views import APIView
from rest_framework.decorators import api_view, permission_classes
from common.strategy.authpermission import IsOrganizer, IsAdmin, IsParticipant
from common.services.service import EventService

# Total CRUD for organizer manage events
class OrganizerEventView(APIView):
    # permission_classes = [IsOrganizer, IsAdmin]

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

# Endpoint for a participant to register for an event
@api_view(['POST'])
@permission_classes([IsAdmin, IsParticipant])
def register_at_event(request):
    pass

@api_view(['GET'])
def process_events(request):
    from core.files import FileGeneration
    from rest_framework.response import Response
    from django.forms.models import model_to_dict

    file_generation = FileGeneration()

    file_generation.generate_event_pdf()
    json_cache = file_generation.get_cache()

    return Response({'events': json_cache}, status=200)

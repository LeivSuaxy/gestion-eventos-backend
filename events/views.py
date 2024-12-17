from rest_framework.views import APIView
from rest_framework.decorators import api_view, permission_classes
from common.strategy.authpermission import IsAdmin, IsParticipant
from common.services.service import EventService, EventProcessedService
from events.services.service import register_at_event as register
from events.services.service import unregister_at_event as unregister

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

class OrganizerProcessEvent(APIView):
    # permission_classes = [IsAdmin, IsOrganizer]

    @staticmethod
    def get(request):
        return EventProcessedService.get()

    @staticmethod
    def delete(request):
        return EventProcessedService.delete(request.data)

# Endpoint for a participant to register for an event
@api_view(['POST'])
@permission_classes([IsAdmin, IsParticipant])
def register_at_event(request):
    return register(request.data, request.user)

@api_view(['POST'])
@permission_classes([IsAdmin, IsParticipant])
def unregister_at_event(request):
    return unregister(request.data, request.user)

@api_view(['GET'])
def process_events(request):
    from core.files import FileGeneration
    from rest_framework.response import Response
    from events.services.service import process_events
    from rest_framework import status

    file_generation = FileGeneration()

    file_id = file_generation.generate_event_pdf()
    json_cache = file_generation.get_cache()

    response_process = process_events(file_id)

    if response_process.status_code != 200:
        return response_process

    return Response({'message': response_process.data, 'data': json_cache}, status=status.HTTP_200_OK)

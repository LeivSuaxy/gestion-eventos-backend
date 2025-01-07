from rest_framework.parsers import MultiPartParser, FormParser
from core.guards.permission_classes import IsAdmin
from common.abstract.apiviews import BaseAdminApiView
from events.services.eventservice import EventService
from administration.api.serializer import EventSerializerAdmin

# Create your views here.
class EventAdminAPIVIEW(BaseAdminApiView):
    permission_classes = [IsAdmin]
    service = EventService()
    parser_classes = (MultiPartParser, FormParser)
    cache_key = 'events'
    object_name_many = 'events'
    object_name_single = 'event'
    serializer_class = EventSerializerAdmin


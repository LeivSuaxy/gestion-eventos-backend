from common.services.base import BaseService
from events.models import Event

class EventService(BaseService):
    model = Event

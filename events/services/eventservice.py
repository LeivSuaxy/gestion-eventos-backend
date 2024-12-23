from common.abstract.service import BaseService
from events.models import Event

class EventService(BaseService):
    model = Event

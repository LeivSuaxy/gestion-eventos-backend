from common.services.base import BaseService
from events.models import Event
from common.patterns.decorators.singleton import singleton

@singleton
class EventService(BaseService):
    model = Event

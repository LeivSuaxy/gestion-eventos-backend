from common.services.base import BaseService
from events.models import Event
from rest_framework.pagination import PageNumberPagination
from django.conf import settings

class PublicEventService(BaseService):
    model = Event

    def __init__(self):
        super().__init__()

    def get_paginated_events(self, request):
        paginator = PageNumberPagination()
        paginator.page_size = settings.REST_FRAMEWORK.get('PAGE_SIZE')
        events = self.model.objects.all()
        return paginator.paginate_queryset(events, request)

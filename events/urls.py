from django.urls import path
from events.api.views import *

urlpatterns = [
    path('oevent/', EventOrganizerAPIVIEW.as_view(), name='organizer_events')
]
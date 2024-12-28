from django.urls import path
from administration.api.views import *

urlpatterns = [
    path('event/', EventAdminAPIVIEW.as_view(), name='event_admin'),
]
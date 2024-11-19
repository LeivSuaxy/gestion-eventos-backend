from django.urls import path
from . import views

urlpatterns = [
    path('test/', views.process_events, name='test'),
]
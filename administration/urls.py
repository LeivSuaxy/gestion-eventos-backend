from django.urls import path
from . import views

urlpatterns = [
    path('events/', views.AdminEventView.as_view(), name='events'),
]
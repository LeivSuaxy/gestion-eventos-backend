from django.contrib import admin
from django.urls import path, include

urlpatterns = [
    path('administration/', admin.site.urls),
    path('authentication/', include('authentication.urls')),
    path('apiadmin/', include('administration.urls')),
    path('apiadmin/events/', include('events.urls')),
]

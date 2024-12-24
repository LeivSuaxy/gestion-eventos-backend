# Script to send notification emails to users when an event is near to start
from celery import shared_task
from django.utils import timezone
from datetime import timedelta
from events.models import Event
from django.core.mail import send_mail

# Build this function
'''@shared_task
def notify_users_about_upcoming_events():
    tomorrow = timezone.now() + timedelta(days=1)
    events = Event.objects.filter(start_date__date=tomorrow.date())
    for event in events:
        users = event.registered_users.all()  # Asumiendo que tienes una relación ManyToMany con los usuarios registrados
        for user in users:
            send_mail(
                'Recordatorio de Evento',
                f'Hola {user.username},\n\nEste es un recordatorio de que el evento "{event.name}" comenzará mañana.',
                'from@example.com',
                [user.email],
                fail_silently=False,
            )'''
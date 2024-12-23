from django.core.mail import send_mail
from django.core.cache import cache
from django.conf import settings

def send_verification_email(user, code):
    subject = 'Your Verification Code'
    message = f'Your verification code is {code}'
    from_email = settings.DEFAULT_FROM_EMAIL
    recipient_list = [user.email]
    send_mail(subject, message, from_email, recipient_list)
    cache.set(user.email, code, timeout=120)
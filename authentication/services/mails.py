from django.core.mail import EmailMultiAlternatives
from django.template.loader import render_to_string
from django.core.cache import cache
from django.conf import settings

def send_verification_email(user, code):
    subject = 'Your Verification Code'
    from_email = settings.DEFAULT_FROM_EMAIL
    recipient_list = [user.email]

    # Render the HTML template with the verification code
    html_content = render_to_string('mails/eventinvitation.html', {'verification_code': code})

    # Create the email
    email = EmailMultiAlternatives(subject, '', from_email, recipient_list)
    email.attach_alternative(html_content, "text/html")
    email.send()

    # Cache the verification code
    cache.set(user.email, code, timeout=120)
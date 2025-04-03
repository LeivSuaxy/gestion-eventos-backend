# Install dependencies.

``
pip install -r requirements.txt
``

# Project Structure

- Apps (Django apps)
  - administration
  - authentication
  - events
  - public
  - streaming

Core and common are not Django apps, they are just Python packages to complement the system.

Public app exists to serve frontend software.

Templates: HTML and CSS files to render emails or other needs.

# App Structure

- urls.py : Define the app's URL patterns.
- models.py : Define the app's data models.
- admin.py : Register the app's models with the Django admin site.
- api:
    - serializers.py : Define serializers for the app's models.
    - views.py : Define API views for the app's models.
- services : Contains the services for the app module.
- tests : Contains the tests for the app module.

# ENVS!
## Redis
- REDIS_HOST : Redis host.
- REDIS_PORT : Redis port.
- USE_REDIS : Boolean(String 'True' or 'False') to indicate if Redis is used or not.

## Rest Framework
- PAGE_SIZE : Default page size for pagination.

## Emails
- EMAIL_HOST : Email host.
- EMAIL_PORT : Email port.
- EMAIL_USE_TLS : Boolean(String 'True' or 'False') to indicate if TLS is used or not.
- EMAIL_HOST_USER : Email host user.
- EMAIL_HOST_PASSWORD : Email host password.
- DEFAULT_FROM_EMAIL : Default from email address.

## Database
- DATABASE_NAME : Database name.
- DATABASE_USER : Database user.
- DATABASE_PASSWORD : Database password.
- DATABASE_HOST : Database host.
- DATABASE_PORT : Database port.

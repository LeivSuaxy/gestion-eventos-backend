# Install dependencies.

``
pip install -r requirements.txt
``

# Project Structure

- Apps (Django apps)
  - administration
  - authentication
  - events

Core and common are not Django apps, they are just Python packages to complement the system.

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


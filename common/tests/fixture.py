import pytest
from rest_framework.test import APIClient
from authentication.models import EventUser
from django.urls import reverse
from uuid import uuid4

user_counter = 0

@pytest.fixture
def api_client():
    return APIClient()

@pytest.fixture
def user_data():
    global user_counter

    user_counter += 1

    return {
        'username': f'testuser{user_counter}_{uuid4()}',
        'email': f'testuser{user_counter}_{uuid4()}@example.com',
        'password': 'testpass123'
    }

@pytest.fixture
def create_user(db, user_data):
    user = EventUser.objects.create_user(
        username=user_data['username'],
        email=user_data['email'],
        password=user_data['password']
    )
    user.is_active=True
    user.save()
    return user

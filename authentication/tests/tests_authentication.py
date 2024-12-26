import pytest
from rest_framework.test import APIClient
from django.urls import reverse

from authentication.models import EventUser


@pytest.fixture
def api_client():
    return APIClient()

@pytest.fixture
def user_data():
    return {
        'username': 'testuser',
        'email': 'testuser@example.com',
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

# authentication/tests/test_authentication.py
@pytest.mark.django_db
def test_register_user(api_client, user_data):
    url = reverse('register')
    response = api_client.post(url, user_data)
    assert response.status_code == 200
    assert response.data['detail'] == 'Verification email sent.'

@pytest.mark.django_db
def test_register_user_existing_email(api_client, create_user, user_data):
    url = reverse('register')
    response = api_client.post(url, user_data)
    assert response.status_code == 400
    assert response.data['detail'] == 'User with this email already exists.'

@pytest.mark.django_db
def test_login_user(api_client, create_user, user_data):
    url = reverse('login')
    response = api_client.post(url, {
        'username': user_data['username'],
        'password': user_data['password']
    })
    assert response.status_code == 200
    assert 'access' in response.data
    assert 'refresh' in response.data

@pytest.mark.django_db
def test_refresh_token(api_client, create_user, user_data):
    login_url = reverse('login')
    refresh_url = reverse('refresh')

    login_response = api_client.post(login_url, {
        'username': user_data['username'],
        'password': user_data['password']
    })
    refresh_token = login_response.data['refresh']

    refresh_response = api_client.post(refresh_url, {
        'refresh': refresh_token
    })
    assert refresh_response.status_code == 200
    assert 'access' in refresh_response.data


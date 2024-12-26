import pytest
from django.urls import reverse
from common.tests.fixture import *
from events.models import Event
from authentication.models import EventUser

@pytest.mark.django_db
@pytest.mark.order(0)
def test_get_empty_events(api_client, create_organizer_token):
    api_client.credentials(HTTP_AUTHORIZATION='Bearer ' + create_organizer_token)
    url = reverse('organizer_events')
    response = api_client.get(url)
    assert response.status_code == 404
    assert response.data == {'message': 'There are not events in database'}


@pytest.fixture
def create_organizer(db, api_client):
    user = EventUser.objects.create_user(
        username='OrganizerTest',
        email='OrganizerTest@gmail.com',
        password='organizer1234',
        role='organizer'
    )

    user.is_active=True
    user.save()

    return user

@pytest.fixture
def create_organizer_token(db, api_client,  create_organizer):
    organizer = create_organizer

    user_data = {
        'username': 'OrganizerTest',
        'password': 'organizer1234'
    }

    url = reverse('login')
    response = api_client.post(url, {
        'username': user_data['username'],
        'password': user_data['password']
    })

    return response.data['access']

@pytest.mark.django_db
@pytest.mark.order(1)
def test_get_events(api_client, create_organizer_token, create_organizer):
    api_client.credentials(HTTP_AUTHORIZATION='Bearer ' + create_organizer_token)
    url = reverse('organizer_events')

    event = Event.objects.create(
        title='Test Event',
        description='Test Description',
        date='2021-01-01',
        organizer=create_organizer
    )

    event.save()

    response = api_client.get(url)

    assert response.status_code == 200
    assert len(response.data) == 1
    assert response.data[0]['title'] == 'Test Event'
    assert response.data[0]['description'] == 'Test Description'
    assert response.data[0]['date'] == '2021-01-01'


class FileGeneration:
    __slots__ = ['__cache__']

    def __init__(self):
        self.__cache__: dict = {}

    def generate_files(self) -> None:
        from events.models import Event, EventParticipant
        from datetime import datetime
        from django.forms.models import model_to_dict
        from authentication.models import User

        events = Event.objects.filter(date__lt=datetime.now())

        if not events:
            return

        for event in events:
            event_object = {
                'title': event.title,
                'description': event.description,
                'date': event.date,
            }
            real_participants: list = []
            participants = EventParticipant.objects.filter(event=event)
            user_details = User.objects.filter(id__in=[participant.user_id for participant in participants])
            for participant in user_details:
                real_participants.append({
                    'id': participant.id,
                    'username': participant.username,
                    'email': participant.email,
                    'first_name': participant.first_name,
                    'last_name': participant.last_name,
                    'phone': participant.phone
                })

            self.__cache__[event.id] = {
                'event': event_object,
                'participants': real_participants
            }


    def get_cache(self):
        return self.__cache__

    def get_json_cache(self):
        import json

        return json.dumps(self.__cache__)
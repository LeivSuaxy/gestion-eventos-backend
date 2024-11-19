class FileGeneration:
    __slots__ = ['__cache__']

    def __init__(self):
        self.__cache__:list = []

    def generate_dict(self) -> None:
        from events.models import Event, EventParticipant
        from datetime import datetime
        from authentication.models import User

        events = Event.objects.filter(date__lt=datetime.now())

        if not events:
            raise Exception('No events found')

        for event in events:
            event_object = {
                'id': event.id,
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

            event_object['participant_quantity'] = participants.count()
            event_object['participants'] = real_participants
            self.__cache__.append(event_object)

    def generate_event_pdf(self) -> None:
        from reportlab.lib.pagesizes import letter
        from reportlab.pdfgen import canvas
        from uuid import uuid4
        from datetime import datetime

        try:
            self.generate_dict()
        except Exception as e:
            print(e)
            return

        uuid = uuid4()
        date = datetime.now()

        filepath = f'./reports/events/{uuid}_{date}_events.pdf'

        c = canvas.Canvas(filepath, pagesize=letter)
        width, height = letter

        y_position = height - 40

        c.setFont("Helvetica-Bold", 24)
        text = 'Events Report'
        text_width = c.stringWidth(text, "Helvetica-Bold", 16)
        c.drawString((width - text_width) / 2.0, y_position, text)

        y_position -= 40

        c.setFont("Helvetica", 16)

        for index, event in enumerate(self.__cache__):
            c.drawString(40, y_position, f'{index+1} - {event["title"]}')
            y_position -= 20
            c.drawString(40, y_position, f'Date: {event["date"]}')
            y_position -= 20

            for participant in event['participants']:
                c.drawString(60, y_position, f'• {participant['first_name']} {participant['last_name']}')
                y_position -= 20

            y_position -= 20

            if y_position < 40:
                c.showPage()
                y_position = height - 40

        c.save()

    def get_cache(self):
        return self.__cache__

    def get_json_cache(self):
        import json

        return json.dumps(self.__cache__)
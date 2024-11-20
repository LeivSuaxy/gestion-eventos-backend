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
                'price': event.price,
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

    def generate_event_pdf(self):
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
        total_money = 0

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

        c.drawString(40, y_position, f'Generated at: {date}')

        y_position -= 40

        c.drawString(40, y_position, f'Report ID: {uuid}')

        y_position -= 60

        for index, event in enumerate(self.__cache__):
            temporal_money = event['price'] * event['participant_quantity']
            c.drawString(40, y_position, f'{index+1} - {event["title"]}')
            y_position -= 20
            c.drawString(40, y_position, f'Date: {event["date"]}')
            y_position -= 20

            for participant in event['participants']:
                c.drawString(60, y_position, f'• {participant['first_name']} {participant['last_name']} Phone: +53 {participant['phone']}')
                y_position -= 20

            total_money += event['price'] * event['participant_quantity']

            y_position -= 20

            c.drawString(40, y_position, f'Recaudado: {temporal_money}$')

            y_position -= 60

            if y_position < 40:
                c.showPage()
                y_position = height - 40

        c.drawString(40, y_position, f'Total Recaudado en todos los eventos: {total_money}$')

        c.save()
        return uuid
    
    def get_cache(self):
        return self.__cache__

    def get_json_cache(self):
        import json

        return json.dumps(self.__cache__)
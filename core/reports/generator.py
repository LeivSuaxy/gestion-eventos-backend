import os
import logging
from reportlab.platypus.flowables import Spacer, Image
from reportlab.platypus import Paragraph
from reportlab.lib.styles import getSampleStyleSheet
from events.models import Event, EventParticipant
import pandas as pd
import matplotlib.pyplot as plt
from reportlab.lib.pagesizes import letter
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle
from reportlab.lib import colors
from authentication.models import EventUser
from django.conf import settings

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Function to generate report of a closed event.
def generate_close_event_report(event: Event):
    try:
        participants = EventParticipant.objects.filter(event=event)
        participant_data = []

        for p in participants:
            user = EventUser.objects.get(id=p.participant_id)
            participant_data.append({
                'name': user.username,
                'registered_at': p.created_at
            })

        # Create a DataFrame
        df = pd.DataFrame(participant_data)

        # Ensure 'registered_at' is in the DataFrame
        if 'registered_at' not in df.columns:
            raise KeyError("'registered_at' column is missing in the DataFrame")

        # Generate a plot of users registered by day
        df['registered_at'] = pd.to_datetime(df['registered_at'])
        df.set_index('registered_at', inplace=True)
        daily_registrations = df.resample('D').size()

        plt.figure(figsize=(10, 6))
        daily_registrations.plot(kind='bar')
        plt.title('User Registrations by Day')
        plt.xlabel('Date')
        plt.ylabel('Number of Registrations')
        plt.tight_layout()
        plot_path = os.path.join(settings.REPORTS_DIR, 'registrations_by_day.png')
        plt.savefig(plot_path)
        plt.close()

        # Generate PDF
        pdf_path = os.path.join(settings.REPORTS_DIR, f'event_{event.id}_report.pdf')
        doc = SimpleDocTemplate(pdf_path, pagesize=letter)
        elements = []

        # Add event name as header
        styles = getSampleStyleSheet()
        header = Paragraph(f'Event: {event.title}', styles['Title'])
        elements.append(header)
        elements.append(Spacer(1, 12))

        # Add table of participants
        table_data = [['Name', 'Registered At']] + [[p['name'], p['registered_at']] for p in participant_data]
        table = Table(table_data)
        table.setStyle(TableStyle([
            ('BACKGROUND', (0, 0), (-1, 0), colors.grey),
            ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
            ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
            ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
            ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
            ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
            ('GRID', (0, 0), (-1, -1), 1, colors.black),
        ]))
        elements.append(table)

        # Add plot image
        elements.append(Spacer(1, 12))
        elements.append(Image(plot_path, width=500, height=300))

        # Add organizer name as footer
        elements.append(Spacer(1, 12))
        footer = Paragraph(f'Organizer: {event.organizer.username}', styles['Normal'])
        elements.append(footer)

        doc.build(elements)

        logger.info(f'Report generated successfully: {pdf_path}')
    except Exception as e:
        logger.error(f'Error generating report: {e}')
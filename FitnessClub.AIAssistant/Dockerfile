FROM python:3.11-slim

WORKDIR /app

# Prevent Python buffering
ENV PYTHONUNBUFFERED=1

# Install dependencies
COPY FitnessClub_Test.AIAssistant/requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

# Copy project
COPY FitnessClub_Test.AIAssistant/ .

EXPOSE 8000

CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
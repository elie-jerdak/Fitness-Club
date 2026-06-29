from fastapi import FastAPI, Depends, HTTPException
from sqlalchemy.orm import Session, joinedload
from typing import List
from database import SessionLocal, engine
from models import Base, Feedback, Coach, Client
from datetime import datetime

from textblob import TextBlob
from vaderSentiment.vaderSentiment import SentimentIntensityAnalyzer
import text2emotion as te
import numpy as np
import re

app = FastAPI()

@app.get("/health")
def health():
    return {"status": "ok"}

# Initialize VADER+
vader = SentimentIntensityAnalyzer()

# Dependency for DB session
def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

# Text preprocessing
def preprocess(text):
    text = text.lower()
    text = re.sub(r'[^\w\s]', '', text)  # Remove punctuation
    return text

# Enhanced sentiment rating function
def get_accurate_stars(comment):
    clean_text = preprocess(comment)

    # Use multiple sentiment techniques
    methods = [
        TextBlob(clean_text).sentiment.polarity,
        vader.polarity_scores(clean_text)['compound'],
        te.get_emotion(clean_text).get('Happy', 0) - te.get_emotion(clean_text).get('Angry', 0)
    ]

    # Domain-specific rules
    weighted = np.mean(methods)
    if 'late' in clean_text:
        weighted -= 0.4
    if 'form' in clean_text:
        weighted += 0.3

    # Convert to 0–5 scale
    return min(5, max(0, round((weighted + 1) * 2.5, 1)))

# Main rating API
@app.get("/rate/{coach_id}")
def get_feedback_rating(coach_id: int, db: Session = Depends(get_db)):

    feedbacks = (
        db.query(Feedback)
        .options(
            joinedload(Feedback.client)
            .joinedload(Client.user)
        )
        .filter(Feedback.CoachId == coach_id)
        .all()
    )

    review_cards = []
    scores = []

    for feedback in feedbacks:
        if feedback.Comment:

            stars = get_accurate_stars(feedback.Comment)
            scores.append(stars)

            user = feedback.client.user if feedback.client else None

            review_cards.append({
                "FeedbackId": feedback.id,
                "ClientName": f"{user.FirstName} {user.LastName}" if user else "Anonymous",
                "ClientProfilePicture": user.Photo if user else None,
                "Stars": stars,
                "Comment": feedback.Comment,
                "Date": feedback.CreatedAt.strftime("%Y-%m-%d") if feedback.CreatedAt else None
            })

    average = round(sum(scores) / len(scores), 2) if scores else 0

    return {
        "coach_id": coach_id,
        "feedback_count": len(scores),
        "average_stars": average,
        "reviews": review_cards
    }
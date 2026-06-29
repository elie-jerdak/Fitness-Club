from sqlalchemy import Column, Integer, String, Float, Text, ForeignKey, Date
from sqlalchemy.orm import relationship
from database import Base
from datetime import date


class AspNetUsers(Base):
    __tablename__ = "AspNetUsers"

    Id = Column(String, primary_key=True)
    FirstName = Column(String)
    LastName = Column(String)
    Photo = Column(String)

    clients = relationship(
        "Client",
        back_populates="user",
        cascade="all, delete-orphan"
    )

    coaches = relationship(
        "Coach",
        back_populates="user",
        cascade="all, delete-orphan"
    )


class Client(Base):
    __tablename__ = "Client"

    ID = Column(Integer, primary_key=True, index=True)
    Height = Column(Float)
    Weight = Column(Float)
    Target = Column(String(255))
    MedicalHistory = Column(String(1000))

    User_ID = Column(String, ForeignKey("AspNetUsers.Id"))

    user = relationship("AspNetUsers", back_populates="clients")

    feedbacks = relationship(
        "Feedback",
        back_populates="client",
        cascade="all, delete-orphan"
    )


class Coach(Base):
    __tablename__ = "Coach"

    ID = Column(Integer, primary_key=True, index=True)
    Experience = Column(String(255))
    Specialty = Column(String(255))
    Salary = Column(Float)
    Bio = Column(Text)

    User_ID = Column(String, ForeignKey("AspNetUsers.Id"))

    user = relationship("AspNetUsers", back_populates="coaches")

    feedbacks = relationship(
        "Feedback",
        back_populates="coach",
        cascade="all, delete-orphan"
    )


class Feedback(Base):
    __tablename__ = "Feedback"

    id = Column(Integer, primary_key=True, index=True)

    CoachId = Column(Integer, ForeignKey("Coach.ID"))
    ClientId = Column(Integer, ForeignKey("Client.ID"))

    Comment = Column(Text)

    CreatedAt = Column(Date, default=date.today)

    coach = relationship("Coach", back_populates="feedbacks")
    client = relationship("Client", back_populates="feedbacks")
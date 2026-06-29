from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, declarative_base
from dotenv import load_dotenv
import os

""" SQL Serer Setup
    DATABASE_URL = (
    "mssql+pyodbc://DESKTOP-8O8T7R7\\SQLEXPRESS/FitnessClubDB_TEST"
    "?driver=ODBC+Driver+18+for+SQL+Server"
    "&trusted_connection=yes"
    "&encrypt=yes"
    "&trustservercertificate=yes"
) 

engine = create_engine(DATABASE_URL)
SessionLocal = sessionmaker(bind=engine, autoflush=False)
Base = declarative_base()

"""

load_dotenv()

DATABASE_URL = os.getenv("DATABASE_URL")

""" Render PostgreSQL URLs often start with: postgres://
But SQLAlchemy expects: postgresql:// """

if DATABASE_URL.startswith("postgres://"):
    DATABASE_URL = DATABASE_URL.replace(
        "postgres://",
        "postgresql://",
        1
    )

engine = create_engine(DATABASE_URL)

SessionLocal = sessionmaker(
    bind=engine,
    autoflush=False,
    autocommit=False
)

Base = declarative_base()
import pyodbc

server_name = r"DESKTOP-8O8T7R7\SQLEXPRESS"  # Use the confirmed instance name
connection_string = (
    f"DRIVER={{ODBC Driver 18 for SQL Server}};"
    f"SERVER={server_name};"
    "Database=FitnessClubDB_TEST;"
    "Trusted_Connection=yes;"  # Windows Authentication
    "Encrypt=yes;"            # Required for ODBC 18
    "TrustServerCertificate=yes;"  # Bypass certificate validation
)

try:
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    cursor.execute("SELECT @@VERSION AS 'SQL Server Version'")
    result = cursor.fetchone()
    print("✅ Successfully connected to SQL Server!")
    print("Version:", result[0])
    conn.close()
except pyodbc.Error as e:
    print("❌ Connection failed:", str(e))
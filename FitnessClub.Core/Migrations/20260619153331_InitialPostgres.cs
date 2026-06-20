using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitnessClub_Test.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DOB = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Photo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    QrCode = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Notification_PK", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlotHeatmap",
                columns: table => new
                {
                    TimeSlot = table.Column<string>(type: "text", nullable: true),
                    Mon = table.Column<int>(type: "integer", nullable: false),
                    Tue = table.Column<int>(type: "integer", nullable: false),
                    Wed = table.Column<int>(type: "integer", nullable: false),
                    Thu = table.Column<int>(type: "integer", nullable: false),
                    Fri = table.Column<int>(type: "integer", nullable: false),
                    Sat = table.Column<int>(type: "integer", nullable: false),
                    Sun = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calendar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Display = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Visibility = table.Column<string>(type: "text", nullable: true),
                    IsBackground = table.Column<bool>(type: "boolean", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CoachID = table.Column<int>(type: "integer", nullable: true),
                    ClientID = table.Column<int>(type: "integer", nullable: true),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    RecurrenceRule = table.Column<string>(type: "text", nullable: true),
                    DaysOfWeek = table.Column<string>(type: "text", nullable: true),
                    EndRecur = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExDates = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    BackgroundColor = table.Column<string>(type: "text", nullable: true),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Calendar_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Calendar_AspNetUsers_ClientID",
                        column: x => x.ClientID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calendar_AspNetUsers_CoachID",
                        column: x => x.CoachID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CheckingInOut",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeIn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    User_ID = table.Column<int>(type: "integer", nullable: false),
                    QrToken = table.Column<string>(type: "text", nullable: true),
                    ScanIp = table.Column<string>(type: "text", nullable: true),
                    ScanDevice = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CheckingInOut_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CheckingInOut_User",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Height = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Target = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MedicalHistory = table.Column<string>(type: "text", nullable: true),
                    User_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Client_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Client_User",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Coach",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Experience = table.Column<int>(type: "integer", nullable: true),
                    Specialty = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Salary = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    User_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Coach_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Coach_User",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Photo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AdminResponse = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    User_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Message_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Message_User",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QrTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    Device = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QrTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToBeNotified",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "integer", nullable: false),
                    Notification_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ToBeNotified_PK", x => new { x.User_ID, x.Notification_ID });
                    table.ForeignKey(
                        name: "FK_ToBeNotified_Notification",
                        column: x => x.Notification_ID,
                        principalTable: "Notification",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ToBeNotified_User",
                        column: x => x.User_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CalendarUser",
                columns: table => new
                {
                    CalendarID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarUser", x => new { x.CalendarID, x.UserID });
                    table.ForeignKey(
                        name: "FK_CalendarUser_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarUser_Calendar_CalendarID",
                        column: x => x.CalendarID,
                        principalTable: "Calendar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Membership",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsAutorenewed = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Client_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Membership_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Membership_Client",
                        column: x => x.Client_ID,
                        principalTable: "Client",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Subscription_Payment",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    StripeSessionId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, defaultValue: "USD"),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Client_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Subscription_Payment_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubscriptionPayment_Client",
                        column: x => x.Client_ID,
                        principalTable: "Client",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Workout_Log",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Client_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Workout_Log_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkoutLog_Client",
                        column: x => x.Client_ID,
                        principalTable: "Client",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Availability",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Day = table.Column<DateOnly>(type: "date", unicode: false, maxLength: 10, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Coach_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Availability_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Availability_Coach",
                        column: x => x.Coach_ID,
                        principalTable: "Coach",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Reccurence = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Fee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxOccupancy = table.Column<int>(type: "integer", nullable: true),
                    Feedback = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Coach_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Class_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Class_Coach",
                        column: x => x.Coach_ID,
                        principalTable: "Coach",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedback_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedback_Coach_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coach",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Premade_Program",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CoverImage = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    EquipmentNeeded = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true),
                    Intensity = table.Column<string>(type: "text", nullable: true),
                    NumberOfExercises = table.Column<int>(type: "integer", nullable: false),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Coach_ID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Premade_Program_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PremadeProgram_Coach",
                        column: x => x.Coach_ID,
                        principalTable: "Coach",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Availability_Booking",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Availability_ID = table.Column<int>(type: "integer", nullable: false),
                    Client_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Availability_Booking_PK", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AvailabilityBooking_Availability",
                        column: x => x.Availability_ID,
                        principalTable: "Availability",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AvailabilityBooking_Client",
                        column: x => x.Client_ID,
                        principalTable: "Client",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Client_ID = table.Column<int>(type: "integer", nullable: false),
                    Class_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Booking_PK", x => x.ID);
                    table.ForeignKey(
                        name: "Booking_Class_FK",
                        column: x => x.Class_ID,
                        principalTable: "Class",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "Booking_Client_FK",
                        column: x => x.Client_ID,
                        principalTable: "Client",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PremadeProgramExercises",
                columns: table => new
                {
                    PremadeProgramId = table.Column<int>(type: "integer", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    NumberOfSets = table.Column<int>(type: "integer", nullable: false),
                    NumberOfReps = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremadeProgramExercises", x => new { x.PremadeProgramId, x.ExerciseId });
                    table.ForeignKey(
                        name: "FK_PremadeProgramExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PremadeProgramExercises_Premade_Program_PremadeProgramId",
                        column: x => x.PremadeProgramId,
                        principalTable: "Premade_Program",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistersIn",
                columns: table => new
                {
                    Premade_Program_ID = table.Column<int>(type: "integer", nullable: false),
                    Client_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("RegistersIn_PK", x => new { x.Premade_Program_ID, x.Client_ID });
                    table.ForeignKey(
                        name: "FK_RegistersIn_Client",
                        column: x => x.Client_ID,
                        principalTable: "Client",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RegistersIn_PremadeProgram",
                        column: x => x.Premade_Program_ID,
                        principalTable: "Premade_Program",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Availability_Coach_ID",
                table: "Availability",
                column: "Coach_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_Booking_Availability_ID_Client_ID",
                table: "Availability_Booking",
                columns: new[] { "Availability_ID", "Client_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Availability_Booking_Client_ID",
                table: "Availability_Booking",
                column: "Client_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Class_ID",
                table: "Booking",
                column: "Class_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Client_ID",
                table: "Booking",
                column: "Client_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_ClientID",
                table: "Calendar",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_CoachID",
                table: "Calendar",
                column: "CoachID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarUser_UserID",
                table: "CalendarUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CheckingInOut_User_ID",
                table: "CheckingInOut",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Class_Coach_ID",
                table: "Class",
                column: "Coach_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Client_User_ID",
                table: "Client",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Coach_User_ID",
                table: "Coach",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_ClientId",
                table: "Feedback",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_CoachId",
                table: "Feedback",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Membership_Client_ID",
                table: "Membership",
                column: "Client_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Message_User_ID",
                table: "Message",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Premade_Program_Coach_ID",
                table: "Premade_Program",
                column: "Coach_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PremadeProgramExercises_ExerciseId",
                table: "PremadeProgramExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_QrTokens_UserId",
                table: "QrTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistersIn_Client_ID",
                table: "RegistersIn",
                column: "Client_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_Payment_Client_ID",
                table: "Subscription_Payment",
                column: "Client_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ToBeNotified_Notification_ID",
                table: "ToBeNotified",
                column: "Notification_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Log_Client_ID",
                table: "Workout_Log",
                column: "Client_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Availability_Booking");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "CalendarUser");

            migrationBuilder.DropTable(
                name: "CheckingInOut");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Membership");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "PremadeProgramExercises");

            migrationBuilder.DropTable(
                name: "QrTokens");

            migrationBuilder.DropTable(
                name: "RegistersIn");

            migrationBuilder.DropTable(
                name: "Subscription_Payment");

            migrationBuilder.DropTable(
                name: "TimeSlotHeatmap");

            migrationBuilder.DropTable(
                name: "ToBeNotified");

            migrationBuilder.DropTable(
                name: "Workout_Log");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Availability");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "Calendar");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Premade_Program");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Coach");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}

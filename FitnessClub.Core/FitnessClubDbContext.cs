using FitnessClub_Test.Dtos;
using FitnessClub_Test.Core.NewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class FitnessClubDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public FitnessClubDbContext()
    {
    }

    public FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Availability> Availabilities { get; set; }

    public virtual DbSet<AvailabilityBooking> AvailabilityBookings { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Calendar> Calendars { get; set; }

    public virtual DbSet<CheckingInOut> CheckingInOuts { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Coach> Coaches { get; set; }

    public virtual DbSet<Feedback> Feedback { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PremadeProgram> PremadePrograms { get; set; }

    public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

    public virtual DbSet<WorkoutLog> WorkoutLogs { get; set; }

    public DbSet<TimeSlotHeatmapDto> TimeSlotHeatmap { get; set; }

    public DbSet<QrToken> QrTokens { get; set; }

    public DbSet<Exercise> Exercises { get; set; }

    public DbSet<PremadeProgramExercise> PremadeProgramExercises { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-8O8T7R7\\SQLEXPRESS;Database=FitnessClubDB_TEST;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TimeSlotHeatmapDto>().HasNoKey();

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Availability_PK");

            entity.ToTable("Availability");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CoachId).HasColumnName("Coach_ID");
            entity.Property(e => e.Day)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Coach).WithMany(p => p.Availabilities)
                .HasForeignKey(d => d.CoachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Availability_Coach");
        });

        modelBuilder.Entity<AvailabilityBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Availability_Booking_PK");

            entity.ToTable("Availability_Booking");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AvailabilityId).HasColumnName("Availability_ID");
            entity.Property(e => e.ClientId).HasColumnName("Client_ID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Availability).WithMany(p => p.AvailabilityBookings)
                .HasForeignKey(d => d.AvailabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvailabilityBooking_Availability");

            entity.HasOne(d => d.Client).WithMany(p => p.AvailabilityBookings)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvailabilityBooking_Client");
            
            entity.HasIndex(e => new { e.AvailabilityId, e.ClientId })
                .IsUnique();
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Booking_PK");

            entity.ToTable("Booking");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.ClassId).HasColumnName("Class_ID");
            entity.Property(e => e.ClientId).HasColumnName("Client_ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Class).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Booking_Class_FK");

            entity.HasOne(d => d.Client).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Booking_Client_FK");
        });

        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Calendar_PK");

            entity.ToTable("Calendar");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.EventTitle)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Calendar ↔ Users many-to-many
        modelBuilder.Entity<Calendar>()
            .HasMany(c => c.Users)
            .WithMany(u => u.AttendingEvents)
            .UsingEntity<Dictionary<string, object>>(
                "CalendarUser",
                j => j.HasOne<User>().WithMany().HasForeignKey("UserID").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Calendar>().WithMany().HasForeignKey("CalendarID").OnDelete(DeleteBehavior.Cascade)
            );

        // Calendar ↔ Coach one-to-many
        modelBuilder.Entity<Calendar>()
            .HasOne(c => c.Coach)
            .WithMany(u => u.CoachEvents)
            .HasForeignKey(c => c.CoachID)
            .OnDelete(DeleteBehavior.Restrict);

        // Calendar ↔ Client one-to-many
        modelBuilder.Entity<Calendar>()
            .HasOne(c => c.Client)
            .WithMany(u => u.ClientEvents)
            .HasForeignKey(c => c.ClientID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CheckingInOut>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("CheckingInOut_PK");

            entity.ToTable("CheckingInOut");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TimeIn).HasColumnType("datetime");
            entity.Property(e => e.TimeOut).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.CheckingInOuts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CheckingInOut_User");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Class_PK");

            entity.ToTable("Class");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CoachId).HasColumnName("Coach_ID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Fee)
                .HasPrecision(18, 2);
            entity.Property(e => e.Reccurence).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Coach).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CoachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Class_Coach");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Client_PK");

            entity.ToTable("Client");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Target).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Clients)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Client_User");

            entity.HasMany(c => c.Feedbacks)
              .WithOne(f => f.Client)
              .HasForeignKey(f => f.ClientId);
        });

        modelBuilder.Entity<Coach>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Coach_PK");

            entity.ToTable("Coach");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Specialty)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Coaches)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Coach_User");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("Feedback");
            entity.HasKey(f => f.Id);

            entity.HasOne(f => f.Coach)
                  .WithMany(c => c.Feedbacks)
                  .HasForeignKey(f => f.CoachId);

            entity.HasOne(f => f.Client)
                  .WithMany(c => c.Feedbacks)
                  .HasForeignKey(f => f.ClientId);

            entity.Property(f => f.CreatedAt)
                  .HasColumnType("date")
                  .HasDefaultValueSql("CAST(GETDATE() AS DATE)");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Membership_PK");

            entity.ToTable("Membership");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClientId).HasColumnName("Client_ID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Client).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membership_Client");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Message_PK");

            entity.ToTable("Message");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Photo).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Time)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notification_PK");

            entity.ToTable("Notification");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Time)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<PremadeProgram>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Premade_Program_PK");

            entity.ToTable("Premade_Program");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CoachId).HasColumnName("Coach_ID");
            entity.Property(e => e.CoverImage).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Coach).WithMany(p => p.PremadePrograms)
                .HasForeignKey(d => d.CoachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PremadeProgram_Coach");

            entity.HasMany(d => d.Clients).WithMany(p => p.PremadePrograms)
                .UsingEntity<Dictionary<string, object>>(
                    "RegistersIn",
                    r => r.HasOne<Client>().WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RegistersIn_Client"),
                    l => l.HasOne<PremadeProgram>().WithMany()
                        .HasForeignKey("PremadeProgramId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RegistersIn_PremadeProgram"),
                    j =>
                    {
                        j.HasKey("PremadeProgramId", "ClientId").HasName("RegistersIn_PK");
                        j.ToTable("RegistersIn");
                        j.IndexerProperty<int>("PremadeProgramId").HasColumnName("Premade_Program_ID");
                        j.IndexerProperty<int>("ClientId").HasColumnName("Client_ID");
                    });
        });

        modelBuilder.Entity<PremadeProgramExercise>()
       .HasKey(x => new { x.PremadeProgramId, x.ExerciseId });

        modelBuilder.Entity<PremadeProgramExercise>()
            .HasOne(x => x.PremadeProgram)
            .WithMany(p => p.PremadeProgramExercises)
            .HasForeignKey(x => x.PremadeProgramId);

        modelBuilder.Entity<PremadeProgramExercise>()
            .HasOne(x => x.Exercise)
            .WithMany(e => e.PremadeProgramExercises)
            .HasForeignKey(x => x.ExerciseId);

        modelBuilder.Entity<SubscriptionPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Subscription_Payment_PK");

            entity.ToTable("Subscription_Payment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ClientId).HasColumnName("Client_ID");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("USD");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Client).WithMany(p => p.SubscriptionPayments)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubscriptionPayment_Client");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Photo).HasMaxLength(255);

            entity.HasMany(d => d.Notifications).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "ToBeNotified",
                    r => r.HasOne<Notification>().WithMany()
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ToBeNotified_Notification"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ToBeNotified_User"),
                    j =>
                    {
                        j.HasKey("UserId", "NotificationId").HasName("ToBeNotified_PK");
                        j.ToTable("ToBeNotified");
                        j.IndexerProperty<int>("UserId").HasColumnName("User_ID");
                        j.IndexerProperty<int>("NotificationId").HasColumnName("Notification_ID");
                    });
        });

        modelBuilder.Entity<WorkoutLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Workout_Log_PK");

            entity.ToTable("Workout_Log");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClientId).HasColumnName("Client_ID");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.Client).WithMany(p => p.WorkoutLogs)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkoutLog_Client");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using FitnessClub.Core.Options;
using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestPDF.Infrastructure;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddDbContext<FitnessClubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPDFGeneratingService, PDFGeneratingService>();
builder.Services.AddScoped<IMvcAuthService, MvcAuthService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ICoachService, CoachService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IPremadeProgramsService, PremadeProgramsService>();
builder.Services.AddScoped<IFastApiService, FastApiService>();
builder.Services.AddScoped<IProfitLossService, ProfitLossService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session expires after 30 minutes of inactivity
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<ApiAuthClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Urls:ApiBaseUrl"]);
});

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<FitnessClubDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Error/{0}"; // Change to a Forbidden View

    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services
    .Configure<StoragePathsOptions>(options =>
{
    var relativePath = builder.Configuration["Paths:ProfileImages"];
    options.ProfileImagesPath = Path.Combine(
        builder.Environment.ContentRootPath,
        relativePath
    );
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

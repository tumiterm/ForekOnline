using AspNetCoreHero.ToastNotification;
using ElecPOE.Contract;
using ElecPOE.Controllers;
using ElecPOE.Data;
using ElecPOE.Middleware;
using ElecPOE.Models;
using ElecPOE.Repository;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/Auth/SignIn";

    option.Cookie.Name = "POEAppCookie";
});

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopLeft;
    config.HasRippleEffect = true;

});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(s =>
{
    s.IdleTimeout = TimeSpan.FromHours(10);
});

builder.Services.AddScoped<IUnitOfWork<StudentAttachment>, StudentAdminRepository>();
builder.Services.AddScoped<IUnitOfWork<AssessmentAttachment>, AssessmentRepository>();
builder.Services.AddScoped<IUnitOfWork<Training>, TrainingRepository>();
builder.Services.AddScoped<IUnitOfWork<Evidence>, EvidenceRepository>();
builder.Services.AddScoped<IUnitOfWork<User>, UserRepository>();
builder.Services.AddScoped<IUnitOfWork<Document>, DocumentRequestRepository>();
builder.Services.AddScoped<IUnitOfWork<ProgressReport>, ResultsRepository>();
builder.Services.AddScoped<IUnitOfWork<Report>, ReportRepository>();
builder.Services.AddScoped<IUnitOfWork<LearnerFinance>, FinanceRepository>();
builder.Services.AddScoped<IUnitOfWork<Course>, CourseRepository>();
builder.Services.AddScoped<IUnitOfWork<Module>, ModuleRepository>();
builder.Services.AddScoped<IUnitOfWork<LessonPlan>, LessonPlanRepository>();
builder.Services.AddScoped<IUnitOfWork<Company>, CompanyRepository>();
builder.Services.AddScoped<IUnitOfWork<Placement>, PlacementRepository>();
builder.Services.AddScoped<IUnitOfWork<ContactPerson>, ContactRepository>();
builder.Services.AddScoped<IUnitOfWork<Address>, AddressRepository>();
builder.Services.AddScoped<IUnitOfWork<NatedEngineering>, NatedRepository>();
builder.Services.AddScoped<IUnitOfWork<LearnerWorkplaceModules>, LearnerModuleRepository>();
builder.Services.AddScoped<IUnitOfWork<Visit>, VisitRepository>();
builder.Services.AddScoped<IUnitOfWork<Medical>, MedicalRepository>();
builder.Services.AddScoped<IUnitOfWork<Application>, ApplicationRepository>();
builder.Services.AddScoped<IUnitOfWork<ApplicationRejection>, RejectionsRepository>();
builder.Services.AddScoped<IUnitOfWork<Guardian>, GuardianRepository>();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<IUnitOfWork<ElecPOE.Models.File>, FileRepository>();
builder.Services.AddScoped<IUnitOfWork<ElecPOE.Models.License>, LicenseRepository>();



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

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseCookiePolicy();

//app.UseMiddleware<CurrentUserMiddleware>();


app.MapControllerRoute(

    name: "default",

    pattern: "{controller=Auth}/{action=SignIn}/{id?}");

app.Run();




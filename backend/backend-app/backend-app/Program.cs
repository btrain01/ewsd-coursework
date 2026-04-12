using backend_app.Attributes;
using backend_app.Configurations;
using backend_app.Context;
using backend_app.Enums;
using backend_app.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.DateFormatString = "HH:mm:ss dd/MM/yyyy";
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//make cdi container aware of auth services
builder.Services.AddExceptionHandler<ExceptionMapper>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthenticationAttribute>();
builder.Services.AddScoped<MeetingService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<DocumentUploadService>();    
builder.Services.AddScoped<AuthenticationUserContext>();
builder.Services.AddSingleton<MessageChannelService>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<MessageChannelService>().GetGlobalWriter());

builder.Services.AddLogging();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSwaggerGen();
var datasourceColumnTranslator = new UpperCaseTranslator();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    var dataSource = @"Host=localhost;Username=postgres;Password=Gamerslife.2;Database=etsystem";

    options
    .UseNpgsql(dataSource, optionsBuilder =>
    {
        optionsBuilder
        .MapEnum<MeetingStatus>("meeting_status", nameTranslator: datasourceColumnTranslator)
        .MapEnum<MeetingType>("meeting_type", nameTranslator: datasourceColumnTranslator)
        .MapEnum<NotificationType>("notification_type", nameTranslator: datasourceColumnTranslator);
    })
    .UseSnakeCaseNamingConvention();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.UseExceptionHandler(_ => { });

app.MapControllers();

app.Run();

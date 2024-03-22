using Business.Implementations;
using Business.Interfaces;
using Common.Configurations;
using Common.Constants;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Implementations;
using Persistence.Interfaces;
using Serilog;
using SuperBarber.Extensions;
using SuperBarber.Filters;
using SuperBarber.Middlewares;
using BarberShopService = Business.Implementations.BarberShopService;

var builder = WebApplication.CreateBuilder(args);
/*
 * ToDo check if email is confirmed before making orders or registering as barber
 * ToDo schedule sending email
 * ToDo save all cities in a state or something like this in react we will need separate endpoint
 * ToDo separate endpoint for getting neighborhoods by city
 * ToDo translate app when we have FE 
 * ToDo FE validations
 */
// Add services to the container.

builder.Services.AddDbContext<SuperBarberDbContext>(
            optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SuperBarber")));

builder.AddRedisCache();

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
builder.Services.Configure<IdentityConfig>(builder.Configuration.GetSection(nameof(IdentityConfig)));
builder.Services.Configure<KafkaEmailProducerConfig>(builder.Configuration.GetSection(nameof(KafkaEmailProducerConfig)));

builder.AddKafkaProducerSingleton();

builder.UseSerilog();

builder.AddCustomAuthentication();

//This is needed in order to use the ValidateModelStateFilter
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.AllowTrailingCommas = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserHandler, UserHandler>();
builder.Services.AddScoped<IBarberShopService, BarberShopService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<IBarberService, BarberService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBarberShopRepository, BarberShopRepository>();
builder.Services.AddScoped<INeighborhoodRepository, NeighborhoodRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IWeekDayRepository, WeekDayRepository>();
builder.Services.AddScoped<IBarberRepository, BarberRepository>();

//Filters
builder.Services.AddMvc(options =>
{
    //options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
    options.Filters.Add<ValidateModelStateFilter>(0);
});

var app = builder.Build();

app.PrepareDataBase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();



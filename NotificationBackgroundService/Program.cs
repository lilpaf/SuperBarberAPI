using NotificationService;
using NotificationService.Extensions;
using NotificationService.Models.Configurations;

var builder = Host.CreateApplicationBuilder(args);

builder.UseSerilog();

builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection(nameof(SmtpConfig)));
builder.Services.Configure<KafkaConsumerConfig>(builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));
builder.Services.Configure<KafkaEmailConsumerConfig>(builder.Configuration.GetSection(nameof(KafkaEmailConsumerConfig)));

builder.Services.AddHostedService<EmailService>();

var host = builder.Build();
host.Run();

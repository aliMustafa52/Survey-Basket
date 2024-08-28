using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Identity;
using Serilog;
using SurveyBasketV3.Api;
using SurveyBasketV3.Api.Middleware;
using SurveyBasketV3.Api.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

//call AddDependencies
builder.Services.AddDependencies(builder.Configuration);

//add Response Caching
builder.Services.AddDistributedMemoryCache();

//add serilog
builder.Host.UseSerilog((context, configration) =>
	configration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
} 

//show Request in logs 
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

//add hangFire (dashboard + Authorization)
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
	DashboardTitle ="Survey Basket",
	Authorization = [
		new HangfireCustomBasicAuthenticationFilter{
			User = app.Configuration.GetValue<string>("HangfireSettings:username"),
			Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
		}
	],
	// Disaple Dasboard actions
	//IsReadOnlyFunc = (DashboardContext context) => true
});

// call SendNewPollNotification method
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollNotification",
			() => notificationService.SendNewPollNotification(null),Cron.Daily);
//before Authorization
app.UseCors();

app.UseAuthorization();

//add Response Caching middleware

app.MapControllers();

app.UseExceptionHandler();

app.UseRateLimiter();

app.MapHealthChecks("health", new HealthCheckOptions
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

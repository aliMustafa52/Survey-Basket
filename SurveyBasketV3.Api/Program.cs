using Microsoft.AspNetCore.Identity;
using SurveyBasketV3.Api;
using SurveyBasketV3.Api.Middleware;
using SurveyBasketV3.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

//call AddDependencies
builder.Services.AddDependencies(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//before Authorization
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();

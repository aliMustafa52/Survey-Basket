using Microsoft.EntityFrameworkCore;
using SurveyBasketV3.Api;
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

app.UseAuthorization();

app.MapControllers();

app.Run();

using SurveyBasketV3.Api;

var builder = WebApplication.CreateBuilder(args);


//call AddDependencies
builder.Services.AddDependencies();


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

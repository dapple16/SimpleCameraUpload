using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var corsPolicy = "CorsPolicy";

builder.Services.AddCors(options =>
{
	var cors = builder.Configuration.GetSection("cors");
	var allowedOrigins = cors.GetSection("allowedOrigins").Get<string[]>();
	options.AddPolicy(corsPolicy, builder =>
	{
		builder
		.WithOrigins(allowedOrigins)
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials();
	});
});


var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference(options =>
	{
		options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
	});
}

app.UseCors(corsPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

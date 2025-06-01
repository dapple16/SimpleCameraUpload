using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Enable detailed logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

// Add services
builder.Services.AddControllers();
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

// Serve static files (needed to serve React frontend from wwwroot)
app.UseDefaultFiles();        // looks for index.html
app.UseStaticFiles();         // serve static assets from wwwroot
app.MapStaticAssets();        // Scalar-specific additional mapping, optional

// Development tools
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference(options =>
	{
		options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
	});
}

// Middlewares
app.UseCors(corsPolicy);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// SPA fallback (React routing)
app.MapFallbackToFile("/index.html");

app.Run();
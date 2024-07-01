using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApiUserManagement.Context;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
var connectionString = builder.Configuration.GetConnectionString("PrimaryConnection");
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(connectionString)
);

var gatewayIPAddress = builder.Configuration["GatewayIPAddress"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Configurar AccesPoint
app.Use(async (context, next) =>
{
    var remoteIPAddress = context.Connection.RemoteIpAddress?.ToString();
    if (remoteIPAddress.StartsWith("::ffff:"))
    {
        remoteIPAddress = remoteIPAddress.Replace("::ffff:", "");
    }
    if (remoteIPAddress != gatewayIPAddress)
    {
        var errorMessage = $"Forbidden: Remote IP Address '{remoteIPAddress}' does not match Gateway IP Address '{gatewayIPAddress}'.";
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync(errorMessage);
        return;
    }

    await next();
});

app.MapControllers();

app.Run();
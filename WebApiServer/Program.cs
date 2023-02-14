using ClientServerComDef;
using Eloe.InterfaceRpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using WebApiServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("TokenAuthority", "");
        options.Audience = "APX";
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                var acceptedTenantId = builder.Configuration.GetValue<string>("AcceptedTenantId", "");
                if (!string.IsNullOrEmpty(acceptedTenantId))
                {
                    var tenantId = ctx?.Principal?.FindFirstValue("tenant_id");
                    if (!acceptedTenantId.Equals(tenantId, StringComparison.CurrentCultureIgnoreCase))
                    {
                        ctx?.Fail($"Invalid tenant_id for this service, only accepted tenant is: {acceptedTenantId}");
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<Eloe.InterfaceSerializer.ILogger, WebApiServer.Logger>();
builder.Services.AddScoped<IInterfaceRpcReceiveCollection, InterfaceRpcReceiveCollection>();
builder.Services.AddScoped<InterfaceExecuteFactory>();

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

using System.Reflection;
using System.Text;
using dotenv.net;
using FluentValidation;
using FluentValidation.AspNetCore;
using Logly.Extensions;
using meerkat;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using SeamlessShareApi;
using SeamlessShareApi.Services;

// load env vars
DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseSentry();

builder.Services.AddScoped<ShareService>();
builder.Services.AddScoped<LinkService>();
builder.Services.AddScoped<TextService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });
builder.Services.AddAuthorization();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = Constants.MaxFileSizeInBytes;
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        corsPolicyBuilder => corsPolicyBuilder
            .WithOrigins("http://localhost:4200", "https://seamless_share.com") // Replace with your allowed origins
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// setup db connection
var dbUrl = builder.Configuration.GetValue<string>("DatabaseUrl");

if (dbUrl is null)
    throw new ArgumentNullException(nameof(dbUrl), "The DatabaseUrl configuration setting is required.");

Meerkat.Connect(dbUrl);

var app = builder.Build();

app.UseLogly(opts => opts
    .AddRequestMethod()
    .AddStatusCode()
    .AddResponseTime()
    .AddUrl()
    .AddResponseLength());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
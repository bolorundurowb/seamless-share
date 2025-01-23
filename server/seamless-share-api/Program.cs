using meerkat;
using SeamlessShareApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseSentry();

builder.Services.AddScoped<ShareService>();
builder.Services.AddScoped<LinkService>();
builder.Services.AddScoped<TextService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// setup db connection
var dbUrl = builder.Configuration.GetValue<string>("DatabaseUrl");

if (dbUrl is null)
    throw new ArgumentNullException(nameof(dbUrl), "The DatabaseUrl configuration setting is required.");

Meerkat.Connect(dbUrl);

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

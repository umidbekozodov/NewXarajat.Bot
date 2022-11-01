using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Context;
using Xarajat.Bot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<XarajatDbContext>(options => 
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<TelegramBotService>();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
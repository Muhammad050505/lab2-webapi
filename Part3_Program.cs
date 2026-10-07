using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<Lab2.WebApi.AppDbContext>(opt =>
    opt.UseSqlite("Data Source=users.db"));

var app = builder.Build();
app.MapControllers();
app.Run();

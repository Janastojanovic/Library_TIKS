using LibraryAPI.Data;
using LibraryAPI.Services;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Konekcija")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5500",
                                    "https://localhost:3000",
                                    "https://localhost:3001",
                                    "https://localhost:5500",
                                    "https://127.0.0.1:5500",
                                    "http://localhost:3000",
                                    "http://localhost:5100",
                                    "https://localhost:5100",
                                    "https://127.0.0.1:5100")
                                    .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                        // policy.AllowAnyOrigin();
                        // policy.AllowAnyHeader();
                    });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ILoanService, LoanService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("CORS");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

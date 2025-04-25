using Microsoft.EntityFrameworkCore;
using UserService.API.Mapping;
using UserService.Application;
using UserService.Data;
using UserService.Data.Repository;

namespace UserService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        string connectionString;
        if (builder.Environment.IsDevelopment())
        {
            connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? string.Empty;
        }
        else
        {
            connectionString = builder.Configuration["ConnectionStrings:MySqlConnection"] ?? string.Empty;
        }

        builder.Services.AddDbContext<UserServiceDbContext>(options =>
        {
            options.UseMySQL(connectionString);
        });
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService.Application.UserService>();
        builder.Services.AddAutoMapper(typeof(UserMapping));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        //}

        app.MapControllers();
        app.UseHttpsRedirection();
        app.Run();
    }
}
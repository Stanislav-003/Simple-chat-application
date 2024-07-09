using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TaskSignalR.BLL.Services;
using TaskSignalR.DAL.Data;
using TaskSignalR.DAL;
using TaskSignalR.BLL.IService;
using TaskSignalR.BLL;
using Microsoft.AspNetCore.SignalR;
using TaskSignalR.BLL.Hubs;
using System.Reflection;

namespace TaskSignalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Test WebCoreApi API",
                    Description = "A simple example ASP.NET Core Web API with SignalR library",
                    Contact = new OpenApiContact
                    {
                        Name = "Stanislav Severin",
                        Email = "severin1975list@gmail.com",
                        Url = new Uri("https://github.com/Stanislav-003?tab=repositories"),
                    },
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole(); 
                loggingBuilder.AddDebug();   
            });

            builder.Services.AddCustomServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}

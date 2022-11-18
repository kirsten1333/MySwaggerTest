using Npgsql;
using MySwaggerTest.Models;
using MySwaggerTest.Controllers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MySwaggerTest
{
    /// <summary>
    /// Класс программа
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Основной метод
        /// </summary>
        public static void Main(string[] args)
        {
            LogsFile.CreateLogFile();
            DBConnect.ConnectDB(new DBConnect());
            WebApplication app = CreateApp(args);
            app.Run(); 
        }

        private static WebApplication CreateApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v01",
                    Title = "VKPost",
                    Description = "Это ASP.NET приложение для получения постов со страницы в ВК и анализа их текстов на количество вхождений одинаковых букв. \n" +
                    "Данные сохраняются в PostGresSQL \n" +
                    "Алгоритм действий: \n" +
                    "1. Post запрос на ввод данных для подключения ДБ Postgress\n" +
                    "2. Post запрос на ввод данных для выбора параметров запроса в ВК\n" +
                    "3. Get запрос на получение данных из VK API.\n" +
                    "4. Get запрос на получение данных из БД",
                    Contact = new OpenApiContact
                    {
                        Name = "Karl LIS VK, TG atc",
                        Url = new Uri("https://vk.com/kirsten1333")
                    }
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            
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
            return app;
        }
    }
}
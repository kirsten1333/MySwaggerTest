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
    /// ����� ���������
    /// </summary>
    public class Program
    {
        /// <summary>
        /// �������� �����
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
                    Description = "��� ASP.NET ���������� ��� ��������� ������ �� �������� � �� � ������� �� ������� �� ���������� ��������� ���������� ����. \n" +
                    "������ ����������� � PostGresSQL \n" +
                    "�������� ��������: \n" +
                    "1. Post ������ �� ���� ������ ��� ����������� �� Postgress\n" +
                    "2. Post ������ �� ���� ������ ��� ������ ���������� ������� � ��\n" +
                    "3. Get ������ �� ��������� ������ �� VK API.\n" +
                    "4. Get ������ �� ��������� ������ �� ��",
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
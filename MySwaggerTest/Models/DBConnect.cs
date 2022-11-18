using Npgsql;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MySwaggerTest.Models
{
    /// <summary>
    /// Настройки подключения к БД PostgressSQL.
    /// </summary>
    public class DBConnect
    {
        /// <summary>
        /// Имя хоста в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public static string Hostname { get; set; } = "localhost";
        /// <summary>
        /// Имя пользователя в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public static string Username { get; set; } = "postgres";
        /// <summary>
        /// Пароль пользователя в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public static string Password { get; set; } = "123";
        /// <summary>
        /// Имя базы данных в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public static string DataBase { get; set; } = "postgres";
        /// <summary>
        /// Готовая строка подключения в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public static string ConnString { get; set; } = $"Host={Hostname};Username={Username};Password={Password};Database={DataBase}";
        /// <summary>
        /// Конструктор ДБ
        /// </summary>
        public DBConnect(string hostname, string username, string password, string database)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
            DataBase = database;
            ConnString = $"Host={Hostname};Username={Username};Password={Password};Database={DataBase}";
        }
        /// <summary>
        /// Подключение к БД с заданными параметрами
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnection ConnectDB()
        {
            NpgsqlConnection nc = new(ConnString);
            nc.Open();
            return nc;
        }
    }
}

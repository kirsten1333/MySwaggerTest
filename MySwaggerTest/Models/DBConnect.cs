using Npgsql;

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
        public string Hostname { get; private set; } = "localhost";
        /// <summary>
        /// Имя пользователя в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public string Username { get; private set; } = "postgres";
        /// <summary>
        /// Пароль пользователя в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public string Password { get; private set; } = "123";
        /// <summary>
        /// Имя базы данных в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public string DataBase { get; private set; } = "postgres";
        /// <summary>
        /// Готовая строка подключения в PostgressSQL
        /// </summary>
        /// <value>String</value>
        public string ConnString { get; private set; } = $"Host=localhost;Username=postgres;Password=123;Database=postgres";
        /// <summary>
        /// Текущее подключение к ДБ
        /// </summary>
        public static NpgsqlConnection? NC{ get; private set; }
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
        /// Конструктор ДБ
        /// </summary>
        public DBConnect()
        {
            Hostname = "localhost";
            Username = "postgres";
            Password = "123";
            DataBase = "postgres";
            ConnString = $"Host={Hostname};Username={Username};Password={Password};Database={DataBase}";
        }
        /// <summary>
        /// Подключение к БД с заданными параметрами
        /// </summary>
        /// <returns></returns>
        public static string ConnectDB(DBConnect dB)
        {
            NC?.Close();
            NC = new(dB.ConnString);
            NC.Open();
            return NC.State.ToString();
        }
    }
}

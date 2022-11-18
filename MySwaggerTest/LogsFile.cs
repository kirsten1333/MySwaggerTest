using System.Reflection;

namespace MySwaggerTest
{
    /// <summary>
    /// Класс управления файлом Лога
    /// </summary>
    public class LogsFile
    {
        static bool IsStart = false;
        static string? LogPath;
        /// <summary>
        /// Создание файла Логов
        /// </summary>
        public static void CreateLogFile()
        {
            DateTime now = DateTime.Now;
            var logFile = $"{Assembly.GetExecutingAssembly().GetName().Name}_{now.Hour}_{now.Minute}_Log.txt";
            //var logFile = $"{Assembly.GetExecutingAssembly().GetName().Name}_Log.txt";
            string path = new(Path.Combine(AppContext.BaseDirectory, "Logs", logFile));
            if (File.Exists(path)) { }
            else { using (File.Create(path)); }
            LogPath = path;
        }
        /// <summary>
        /// Запись в файл Лога строки
        /// </summary>
        /// <param name="s">Строка для записи в файл</param>
        public static void Log(string s)
        {
            StreamWriter streamWriter = new(LogPath!, true);
            streamWriter.WriteLine(s);
            streamWriter.WriteLine();
            streamWriter.Close();
        }
        /// <summary>
        /// Запись в файл начала или конца обработки запроса
        /// </summary>
        public static void LogCheck()
        {
            StreamWriter streamWriter = new(LogPath!, true);
            if (!IsStart) { streamWriter.WriteLine("Начало подсчёта данных: "); IsStart = true; }
            else { streamWriter.WriteLine("Конец подсчёта данных: "); IsStart = false; }
            streamWriter.Write(DateTime.Now.ToString());
            streamWriter.WriteLine();
            streamWriter.Close();
        }
    }
}

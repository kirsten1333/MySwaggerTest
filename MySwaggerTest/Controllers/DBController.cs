using Microsoft.AspNetCore.Mvc;
using MySwaggerTest.Models;
using Npgsql;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MySwaggerTest.Controllers
{
    /// <summary>
    /// Контроллер взаимодействия с базой данных
    /// </summary>
    [Route("DB API/[controller]")]
    [ApiController]
    public class DBController : ControllerBase
    {
        DBConnect dBConnect = new DBConnect(DBConnect.Hostname, DBConnect.Username, DBConnect.Password, DBConnect.DataBase);
        NpgsqlConnection nc = new();

        // GET: DB api/<DBController>

        /// <summary>
        /// Этот метод выводит все данные подключенной БД PostGress.
        /// </summary>
        /// <returns>Вывод таблицы из БД</returns>
        /// <response code="500">Проблема в подключении к БД</response>
        /// <response code="200">Успех</response>
        [HttpGet]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DBDic>>> GetFromDB()
        {
            nc = dBConnect.ConnectDB();
            NpgsqlCommand npgcGetAll = new NpgsqlCommand(
                "SELECT * FROM public.DicOfPost " +
                "ORDER BY symbol ASC;", nc);

            await using (var reader = await npgcGetAll.ExecuteReaderAsync())
            {
                List<DBDic> dicList = new List<DBDic>();
                if (reader.HasRows)//Если пришли результаты
                {
                    while (await reader.ReadAsync())
                    {
                        char symbol = Convert.ToChar(reader[0]);
                        int countOfIn = reader.GetInt32(1);
                        dicList.Add(new DBDic(symbol, countOfIn));
                    }
                    return dicList;
                }
                else return NotFound();
            } 
        }

        // GET: DB api/<DBController>/ID
        /// <summary>
        /// Этот метод выводит количство вхождений буквы в подключенной таблице БД PostGress.
        /// </summary>
        /// <returns>Вывод количества вхождений символа</returns>
        /// <param name="id">Char символ количство взхождений которого надо показать</param>
        /// <response code="500">Проблема в подключении к БД</response>
        /// <response code="200">Успех</response>
        /// <response code="404">Нет данных</response>
        [HttpGet("{id}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<string> Get(char id)
        {
            try { nc = dBConnect.ConnectDB(); }
            catch(Exception) { Response.StatusCode = 500; return BadRequest(); }
            //nc = dBConnect.ConnectDB();
            NpgsqlCommand npgcGetId = new NpgsqlCommand(
                "SELECT * FROM public.DicOfPost;", nc);
            var reader = npgcGetId.ExecuteReader();

            if (reader.HasRows)//Если пришли результаты
            {
                while (reader.Read())
                {
                    if (reader.GetChar(0) == id)
                    {
                        string respon = $"Count of \"{reader[0]}\" symbols in posts == {reader.GetInt32(1)}";
                        return respon;
                    }    
                }
            }
            return NotFound();
        }

        // POST: DB api/<DBController>
        /// <summary>
        /// Этот метод опредлят настройки подключения к БД PostGress.
        /// </summary>
        /// <returns>Исход действия</returns>
        /// <param name="hostname">Название Хоста БД</param>
        /// <param name="username">Имя пользователя БД</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="dataBase">Название БД</param>
        [HttpPost]
        public ActionResult<string> Post(string hostname = "localhost", string username = "Server", string password = "123", string dataBase = "VKPostDB")
        {
            try
            {
                DBConnect.Hostname = hostname;
                DBConnect.Username = username;
                DBConnect.Password = password;
                DBConnect.DataBase = dataBase;
                DBConnect.ConnString = $"Host={hostname};Username={username};Password={password};Database={dataBase}";
                return $"Success connection DB has set to {DBConnect.ConnString}";
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: DB api/<DBController>
        /// <summary>
        /// Этот метод удаляет таблицу из подключенной БД.
        /// </summary>
        /// <returns>Исход действия</returns>
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteTodoItem()
        {
            nc = dBConnect.ConnectDB();
            NpgsqlCommand npgcGetAll = new NpgsqlCommand(
                "DROP TABLE IF EXISTS DicOfPost;", nc);
            var reader = await npgcGetAll.ExecuteReaderAsync();
            return "Succesfully deleted if exists before"; 
            
        }
    }


}

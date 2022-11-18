using Microsoft.AspNetCore.Mvc;
using MySwaggerTest.Models;
using Npgsql;
using System.ComponentModel.DataAnnotations;

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
        // GET: DB api/<DBController>
        /// <summary>
        /// Этот метод выводит все данные подключенной БД PostGress.
        /// </summary>
        /// <returns>Вывод таблицы из БД</returns>
        /// <response code="401">Проблема в подключении к БД</response>
        /// <response code="200">Успех</response>
        [HttpGet]
        [ProducesResponseType(401)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DBDic>>> GetFromDB()
        {
            if(DBConnect.NC == null || DBConnect.NC!.State.ToString() != "Open") 
            {
                Response.StatusCode = 401;
                return BadRequest("Не удалось подключиться к базе данных. Проверьте введённые данные. Статус подключения: {DBConnect.NC.FullState}");
            }
            NpgsqlCommand npgcGetAll = new NpgsqlCommand(
                "SELECT * FROM public.DicOfPost " +
                "ORDER BY symbol ASC;", DBConnect.NC);

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
                    return Ok(dicList);
                }
                else return NotFound("База данных пуста, загрузите данные методом POST");
            } 
        }

        // GET: DB api/<DBController>/ID
        /// <summary>
        /// Этот метод выводит количство вхождений буквы в подключенной таблице БД PostGress.
        /// </summary>
        /// <returns>Вывод количества вхождений символа</returns>
        /// <param name="id">Char символ количество вхождений которого надо показать</param>
        /// <response code="500">Проблема в подключении к БД</response>
        /// <response code="200">Успех</response>
        /// <response code="404">Нет данных</response>
        [HttpGet("{id}")]
        [ProducesResponseType(401)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<string> Get(char id)
        {
            if (DBConnect.NC == null || DBConnect.NC!.State.ToString() != "Open")
            {
                Response.StatusCode = 401;
                return BadRequest("Не удалось подключиться к базе данных. Проверьте введённые данные. Статус подключения: {DBConnect.NC.FullState}");
            }
            NpgsqlCommand npgcGetId = new NpgsqlCommand(
                "SELECT * FROM public.DicOfPost;", DBConnect.NC);
            var reader = npgcGetId.ExecuteReader();

            if (reader.HasRows)//Если пришли результаты
            {
                while (reader.Read())
                {
                    if (reader.GetChar(0) == id)
                    {
                        string respon = $"Count of \"{reader[0]}\" symbols in posts == {reader.GetInt32(1)}";
                        return Ok(respon);
                    }    
                }
            }
            return NotFound($"Вхождений символа {id} - не найдено в ДБ");
        }

        // POST: DB api/<DBController>
        /// <summary>
        /// Этот метод опредлят настройки подключения к БД PostGress.
        /// </summary>
        /// <returns>Исход действия</returns>
        /// <response code="200">Успех</response>
        /// <response code="400">Не введены данные /введены неврные данные</response>
        /// <response code="401">Проблема в подключении к БД</response>
        /// <param name="hostname">Имя хоста</param>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="database">Названи базы данных</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "Hostname": "localhost",
        ///        "Username": "postgres",
        ///        "Password": "123",
        ///        "DataBase": "postgres"
        ///     }
        ///
        /// </remarks>

        [HttpPost]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Post(string hostname, string username, string password, string database)
        {
            DBConnect dB = new(hostname, username, password, database);  
            try { DBConnect.ConnectDB(dB); }
            catch {
                Response.StatusCode = 401;
                return BadRequest($"Не удалось подключиться к базе данных. Проверьте введённые данные. Статус подключения: {DBConnect.NC!.FullState}");
            }   
            if (DBConnect.NC!.State.ToString() == "Open")
            {
                return Ok($"Connection status : {DBConnect.NC.FullState}. With parametrs: {dB.ConnString}");
            }
            return BadRequest($"Неизвстная ошибка статус подключения: {DBConnect.NC.FullState}");
        }

        // DELETE: DB api/<DBController>
        /// <summary>
        /// Этот метод удаляет таблицу из подключенной БД.
        /// </summary>
        /// <returns>Исход действия</returns>
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteTodoItem()
        {
            NpgsqlCommand npgcGetAll = new NpgsqlCommand(
                "DROP TABLE IF EXISTS DicOfPost;", DBConnect.NC);
            using var reader = await npgcGetAll.ExecuteReaderAsync();
            return "Succesfully deleted if exists before"; 
        }
    }


}

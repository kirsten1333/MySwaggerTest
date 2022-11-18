using Microsoft.AspNetCore.Mvc;
using MySwaggerTest.Models;
using Npgsql;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;

namespace MySwaggerTest.Controllers
{
    /// <summary>
    /// Контроллер взаимодействия с VK API
    /// </summary>
    [Route("VK API/[controller]")]
    [ApiController]
    public class VKController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        // GET: api/<DBController>
        /// <summary>
        /// Этот метод получает посты из ВК, обрабатывает и загружает данные в БД.
        /// </summary>
        /// <returns>Вывод сырых данных последних постов или неудачный исход</returns>
        /// <response code="200">Успех</response>
        /// <response code="400">Не введены данные /введены неврные данные для подключния к ДБ</response>
        /// <response code="401">Проблема в подключении к БД</response>
        /// <response code="402">База данных заполнена</response>
        /// <response code="500">Проблема в подключении к VK API, попробуйте ввести другие данные для запроса</response>
        /// /// <response code="500">Access Token отсутствует введите его методом POST</response>
        [HttpGet]
        [ProducesResponseType(501)]
        [ProducesResponseType(500)]
        [ProducesResponseType(402)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        
        public async Task<ActionResult<string>> GetFromVkInDB()
        {
            if (VKGet.access_token == null)
            {
                Response.StatusCode = 401;
            }
            if (DBConnect.NC == null || DBConnect.NC!.State.ToString() != "Open")
            {
                Response.StatusCode = 401;
                return BadRequest("Не удалось подключиться к базе данных. Проверьте введённые данные. Статус подключения: {DBConnect.NC.FullState}");
            }
            LogsFile.LogCheck();
            string jsonResponse = await GetRequestAsync(VKGet.QuaeryString);
            StringBuilder stringBuilder;
            try
            {
                stringBuilder = GetFromVK(jsonResponse);
            }
            catch
            {
                LogsFile.Log("ERROR FROM VK API:" + jsonResponse);
                LogsFile.LogCheck();
                Response.StatusCode = 500;
                return "ERROR FROM VK API: " + jsonResponse; 
            }
           
            var dic = DicCreate.GetDictionary(stringBuilder.ToString().ToLower());

            NpgsqlCommand npgc = new NpgsqlCommand(
                "CREATE TABLE IF NOT EXISTS DicOfPost (" +
                "Symbol VARCHAR(1) PRIMARY KEY," +
                "CountOfIn INT NOT NULL);", DBConnect.NC);

            int rows_changed = npgc.ExecuteNonQuery();

            foreach (var item in dic)
            {
                string quaeryADD =
                "INSERT INTO DicOfPost " +
                $"Values (\'{item.Key}\',{item.Value});";
                
                NpgsqlCommand addSymbol = new NpgsqlCommand(
                quaeryADD, DBConnect.NC);
                try { int rows_changed2 = addSymbol.ExecuteNonQuery(); }
                catch 
                {
                    LogsFile.Log("Error __________ DB is FULL");
                    LogsFile.LogCheck();
                    Response.StatusCode = 402;
                    return BadRequest("DB is FULL, delete old DATA"); 
                }
            }
            LogsFile.Log("Success! Raw data: " + stringBuilder.ToString());
            LogsFile.LogCheck();
            return Ok("Success! Raw data: " + stringBuilder.ToString());
        }

        private static StringBuilder GetFromVK(string jsonResponse)
        {
            JsonNode forecastNode = JsonNode.Parse(jsonResponse)!;
            var postNode = forecastNode!["response"]!["items"]!;
            StringBuilder stringBuilder = new();
            foreach (var post in postNode.AsArray())
            {
                stringBuilder.Append(post!["text"]!);
            }

            return stringBuilder;
        }

        // POST: VK api/<VKController>
        /// <summary>
        /// Этот метод опредлят запрос к API VK.
        /// </summary>
        /// <returns>Исход действия</returns>
        /// <param name="access_token">Access_token можно получить в API VK следуя инструкции на официальном сайте 
        /// <see href="https://www.pandoge.com/socialnye-seti-i-messendzhery/poluchenie-klyucha-dostupa-access_token-dlya-api-vkontakte"/></param>
        /// <see href="https://www.pandoge.com/socialnye-seti-i-messendzhery/poluchenie-klyucha-dostupa-access_token-dlya-api-vkontakte"/>
        /// <param name="owner_id">ID пользователя ВК</param>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Post(
        string access_token = "vk1.a.9X3O6CNcnFMfGbKkzFnFouyrSU5o0IJcP4gEveJLijATDUF1sLJTbYpu9hmFAi1TyneLVf1pJ1qQj1R2RQhIG_L4 - tU3I0pirPF2J6HcP_9OyLSAGZ7vWWZDNYDSz_Ll4ow8_gFKkOEwJ_uLI7qyb_YIMsBuSTy1vQwLMnUkgNokNCfRvEedpWbeW4BDZYLIO6HupwC_MADuZgXDrZpgeQ",
        string owner_id = "-1")
        {
            try
            {
                VKGet.access_token = access_token;
                VKGet.owner_id = owner_id;
                VKGet.Quaery = $"?access_token={access_token}&owner_id={owner_id}&count={VKGet.count}&v={VKGet.v}";
                VKGet.QuaeryString = $"{VKGet.path}" + "/" + VKGet.Quaery;
                return $"Success access_token set to{access_token}, owner_id set to {owner_id}";
            }
            catch (Exception)
            {
                return BadRequest("error");
            }
        }
        private static async Task<string> GetRequestAsync(string url)
        {
            using HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}

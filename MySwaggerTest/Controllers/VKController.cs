using Microsoft.AspNetCore.Mvc;
using MySwaggerTest.Models;
using Npgsql;
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
        readonly DBConnect dBConnect = new DBConnect(DBConnect.Hostname, DBConnect.Username, DBConnect.Password, DBConnect.DataBase);
        NpgsqlConnection nc = new();

        // GET: api/<DBController>
        /// <summary>
        /// Этот метод получает посты из ВК, обрабатывает и загружает данные в БД.
        /// </summary>
        /// <returns>Вывод сырых данных последних постов или неудачный исход</returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetFromVkInDB()
        {
            LogsFile.LogCheck();
            nc = dBConnect.ConnectDB();
            string jsonResponse = await GetRequestAsync(VKGet.QuaeryString);
            StringBuilder stringBuilder;
            try
            {
                stringBuilder = GetFromVK(jsonResponse);
            }
            catch
            {
                LogsFile.Log("Error" + jsonResponse);
                LogsFile.LogCheck();
                return "ERROR: " + jsonResponse; 
            }
           
            var dic = DicCreate.GetDictionary(stringBuilder.ToString().ToLower());

            NpgsqlCommand npgc = new NpgsqlCommand(
                "CREATE TABLE IF NOT EXISTS DicOfPost (" +
                "Symbol VARCHAR(1) PRIMARY KEY," +
                "CountOfIn INT NOT NULL);", nc);

            int rows_changed = npgc.ExecuteNonQuery();

            foreach (var item in dic)
            {
                string quaeryADD =
                "INSERT INTO DicOfPost " +
                $"Values (\'{item.Key}\',{item.Value});";
                
                NpgsqlCommand addSymbol = new NpgsqlCommand(
                quaeryADD, nc);
                try { int rows_changed2 = addSymbol.ExecuteNonQuery(); }
                catch 
                {
                    LogsFile.Log("Error __________ DB is FULL");
                    LogsFile.LogCheck();
                    Response.StatusCode = 500;
                    return BadRequest(); 
                }
            }
            LogsFile.Log("Success! Raw data: " + stringBuilder.ToString());
            LogsFile.LogCheck();
            return "Success! Raw data: " + stringBuilder.ToString();
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
        /// <param name="access_token">Access_token можно получить в API VK следуя инструкции на официальном сайте </param>
        /// <see href="https://www.pandoge.com/socialnye-seti-i-messendzhery/poluchenie-klyucha-dostupa-access_token-dlya-api-vkontakte"/>
        /// <param name="owner_id">Имя пользователя БД</param>

        [HttpPost]
        public ActionResult<string> Post(
        string access_token = "vk1.a.9X3O6CNcnFMfGbKkzFnFouyrSU5o0IJcP4gEveJLijATDUF1sLJTbYpu9hmFAi1TyneLVf1pJ1qQj1R2RQhIG_L4 - tU3I0pirPF2J6HcP_9OyLSAGZ7vWWZDNYDSz_Ll4ow8_gFKkOEwJ_uLI7qyb_YIMsBuSTy1vQwLMnUkgNokNCfRvEedpWbeW4BDZYLIO6HupwC_MADuZgXDrZpgeQ",
        string owner_id = "-1")
        {
            try
            {
                VKGet.access_token = access_token;
                VKGet.owner_id = owner_id;
                return $"Success access_token set to{access_token}, owner_id set to {owner_id}";
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        private static async Task<string> GetRequestAsync(string url)
        {
            using HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}

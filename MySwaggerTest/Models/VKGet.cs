using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MySwaggerTest.Models
{
    /// <summary>
    /// Настройки запроса к API vk.com
    /// </summary>
    /// <see href="https://dev.vk.com/reference"/>
    public static class VKGet
    {
        /// <summary>
        /// Access_token и подробности его получения
        /// </summary>
        /// <see href="https://www.pandoge.com/socialnye-seti-i-messendzhery/poluchenie-klyucha-dostupa-access_token-dlya-api-vkontakte"/>
        [Required]
        public static string? access_token { get; set; } 
        /// <summary>
        /// ID страницы в VK с которой будут браться посты
        /// </summary>
        [Required]
        public static string owner_id { get; set; } = "-1";
        /// <summary>
        /// Количество постов взятых с страницы в ВК
        /// </summary>
        public static string count { get; set; } = "5";
        /// <summary>
        /// Версия API VK
        /// </summary>
        public static string v { get; set; } = "5.131";
        /// <summary>
        /// Путь, указывающий метод API VK (wall.get) - получение постов со стены
        /// </summary>
        public static string path { get; set; } = "https://api.vk.com/method/wall.get";
        /// <summary>
        /// Запрос составленный на основе данных
        /// </summary>
        public static string Quaery { get; set; } = $"?access_token={access_token}&owner_id={owner_id}&count={count}&v={v}";
        /// <summary>
        /// Конечное обращение в ВК
        /// </summary>
        public static string QuaeryString { get; set; } = $"{path}" + "/" + Quaery;
    }
}

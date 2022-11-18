using System.Text.RegularExpressions;

namespace MySwaggerTest
{
    /// <summary>
    /// Класс для работы со словарём
    /// </summary>
    public static class DicCreate
    {
        /// <summary>
        /// Метод, создающий словарь, подсчитывающий входящие в строке буквы
        /// </summary>
        /// <param name="words">Строка в которой будут считаться вхождения</param>
        /// <returns>Словарь букв с количеством вхождений каждой</returns>
        public static Dictionary<char, int> GetDictionary(string words)
        {
            Dictionary<char, int> letters = new();
            foreach (var word in words)
            {
                if (Regex.IsMatch(word.ToString(), @"\w", RegexOptions.IgnoreCase)
                    && Regex.IsMatch(word.ToString(), @"\D")
                    && word != '_')
                {
                    if (letters.ContainsKey(word))
                    {
                        letters[word]++;
                    }
                    else { letters.Add(word, 1); }
                }
            }
            return letters;
        }
    }
}

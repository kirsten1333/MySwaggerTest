namespace MySwaggerTest.Models
{
    /// <summary>
    /// Структура БД.
    /// </summary>
    public class DBDic
    {

        /// <summary>
        /// Первичный ключ CHAR. Только алфавитные символы.
        /// </summary>
        /// <value>CHAR</value>
        public char Symbol { get; set; }
        /// <summary>
        /// Количество вхождений того или иного символа.
        /// </summary>
        /// <value>Integer >0 </value>
        public int CountOfIn { get; set; }
        /// <summary>
        /// Строка таблицы в БД.
        /// </summary>
        public DBDic(char symbol, int countOfIn) {
        Symbol= symbol;
        CountOfIn = countOfIn;
        }
    }
}

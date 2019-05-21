using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;



namespace clientWordSearcher
{  
    /// <summary>
    /// Предоставляет функционал для управления игрой
    /// </summary>
    class GameAPI 
    {
        //адрес сервера
        private string baseAdress;

        //ключ авторизации
        private string authCode;

        //состояние игровой сессии
        private bool gameIsStarted = false;

        /// <summary>
        /// Инициализирует экземпляр игрового API 
        /// </summary>
        /// <param name="url">URI сервера на котром находится игровой сервис</param>
        /// <param name="authorizationCode">Код авторизации</param>
        public GameAPI(string url, string authorizationCode)
        {
            baseAdress = url;
            authCode = authorizationCode;
        }

        /// <summary>
        /// Метод инициализирует игровую сессию, возвращает видимую площадку
        /// </summary>
        /// <returns>Возвращает площадку видимую сразу после инициализации</returns>
        public Area StartGame()
        {
            var response = WebAPI.RequestPOST(baseAdress + "/task/game/start", "", authCode);
            
            gameIsStarted = true;
            var area = new Area(response);
            return area;
        }

        /// <summary>
        /// Если игровая сессия инициализирована возвращает текущую игровую статистику
        /// </summary>
        /// <returns></returns>
        public Statistics GetStatistics()
        {
            if (gameIsStarted)
            {
                var response = WebAPI.RequestGET(baseAdress + "/task/game/stats", authCode);

                var statistics = JsonConvert.DeserializeObject<Statistics>(response);

                return statistics;
            }
            else
            {
                Console.WriteLine("Игровая сессия не инициализирована");
                return null;
            }
        }

        /// <summary>
        /// Если игровая сессия инициализирована 
        /// метод выполняет ход в заданную сторону, возвращает площадку видимую после хода
        /// </summary>
        /// <param name="direction">Указывает направление хода</param>
        /// <returns></returns>
        public Area Move(string direction)
        {
            if (gameIsStarted)
            {
                var response = WebAPI.RequestPOST(baseAdress + "/task/Move/" + direction, "", authCode);
                var area = new Area(response);
                return area;
            }
            else
            {
                Console.WriteLine("Игровая сессия не инициализирована");
                return null;
            }
        }

        /// <summary>
        /// Если игровая сессия инициализирована
        /// отправляет строку содержащую набор слов в JSON-формате,
        /// возвращает число полученных очков
        /// </summary>
        /// <param name="words">Список слов в JSON-формате</param>
        /// <returns></returns>
        public Statistics SendWords(string words)
        {

            if (gameIsStarted)
            {
                byte[] utf8JsonWords = Encoding.UTF8.GetBytes(words);
                var response = WebAPI.RequestPOST(baseAdress + "/task/words/", utf8JsonWords, authCode);

                var statistics = JsonConvert.DeserializeObject<Statistics>(response);
                return statistics;
            }
            else
            {
                Console.WriteLine("Игровая сессия не инициализирована");
                return null;
            }
        }

        /// <summary>
        /// Если игровая сессия инициализирована,
        /// завершает игру завершает игровую сессию, возвращает итоговую статистику 
        /// </summary>
        /// <returns></returns>
        public Statistics EndGame()
        {
            if (gameIsStarted)
            {
                var response = WebAPI.RequestPOST(baseAdress + "/task/game/finish", "", authCode);
                gameIsStarted = false;
                var statistics = JsonConvert.DeserializeObject<Statistics>(response);
                Console.WriteLine("End");
                return statistics;
            }
            else
            {
                Console.WriteLine("Игровая сессия не инициализирована");
                return null;
            }
        }
    }
}

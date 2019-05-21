using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace clientWordSearcher
{   
    static class WebAPI  //класс упрощающий использование HTTP-запросов
    {
        /// <summary>
        /// Содержит заголовки ответа сервера
        /// </summary>
        public static WebHeaderCollection responseHeaders;
        
       /// <summary>
       /// Отсылает POST-запрос и возвращает ответ от сервера
       /// </summary>
       /// <param name="adress">URI сервера</param>
       /// <param name="param">Строка параметров</param>
       /// <param name="authCode">Код авторизации</param>
       /// <returns></returns>
        public static string RequestPOST(string adress, string param, string authCode)
        {
          
            using (var client = new WebClient())
            {
                //client.QueryString.Add("test","true");
                client.Headers.Add("Authorization: token " + authCode);
                client.Headers.Add("Content-Type: application/json");

                var response = client.UploadString(adress, "POST", param);
                responseHeaders = client.ResponseHeaders;

                return response;
            }
           
        }

        /// <summary>
        /// Отсылает POST-запрос и возвращает ответ от сервера
        /// </summary>
        /// <param name="adress">URI сервера</param>
        /// <param name="param">Массив байт параметров</param>
        /// <param name="authCode">Код авторизации</param>
        /// <returns></returns>
        public static string RequestPOST(string adress, byte[] param, string authCode)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Authorization: token " + authCode);
                client.Headers.Add("Content-Type: application/json");

                byte[] response = client.UploadData(adress, "POST", param);
                responseHeaders = client.ResponseHeaders;

                return Encoding.UTF8.GetString(response);
            }
        }


       /// <summary>
       /// Отсылает GET-запрос и возвращает ответ отсервера
       /// </summary>
       /// <param name="adress">URI сервера</param>
       /// <param name="authCode">Код авторизации</param>
       /// <returns></returns>
        public static string RequestGET(string adress, string authCode)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Authorization: token " + authCode);

                var response = client.DownloadString(adress);
                responseHeaders = client.ResponseHeaders;
                return response;
            }
        }
    }
}

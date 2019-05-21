using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace clientWordSearcher
{
    class Program
    {
        /// <summary>
        /// Инициализирует алфавит строками представляющими буквы
        /// </summary>
        private static void InitABC(Dictionary<String, char> ABC)
        {
            //одинаковые строки обозначающие разные символы закомментированы
            //вся буква, 7 клеток высота, буквы которые можно определить только полным сканированием
            ABC.Add("1111111\r\n1000000\r\n1000000\r\n1000000\r\n1000000\r\n1000000\r\n1000000", 'Г');
            ABC.Add("1111111\r\n1000000\r\n1000000\r\n1000000\r\n1000000\r\n1000000\r\n1111111", 'С');
            ABC.Add("1001001\r\n1001001\r\n1001001\r\n1001001\r\n1001001\r\n1001001\r\n1111111", 'Ш');
            ABC.Add("1001001\r\n1001001\r\n1001001\r\n1001001\r\n1001001\r\n1111111\r\n0000001", 'Щ');

            //без двух нижних строк, 5 клеток высота, буквы которые однозначно можно определить 5 строками
            ABC.Add("0001000\r\n0010100\r\n0010100\r\n0111110\r\n0100010", 'А');
            ABC.Add("1111111\r\n1000000\r\n1000000\r\n1111111\r\n1000001", 'Б');
            ABC.Add("1111111\r\n1000001\r\n1000001\r\n1111110\r\n1000001", 'В');
            //ABC.Add("1111111\r\n1000000\r\n1000000\r\n1000000\r\n1000000", 'Г');
            ABC.Add("0111110\r\n0100010\r\n0100010\r\n0100010\r\n0111110", 'Д');
            ABC.Add("1111111\r\n1000000\r\n1000000\r\n1111111\r\n1000000", 'Е');
            ABC.Add("0100010\r\n0000000\r\n1111111\r\n1000000\r\n1111111", 'Ё');
            ABC.Add("1001001\r\n0101010\r\n0011100\r\n0001000\r\n0011100", 'Ж');
            ABC.Add("1111111\r\n0000001\r\n0000001\r\n1111110\r\n0000001", 'З');
            ABC.Add("1000001\r\n1000011\r\n1000101\r\n1001001\r\n1010001", 'И');
            ABC.Add("1011101\r\n1000011\r\n1000101\r\n1001001\r\n1010001", 'Й');
            ABC.Add("1000001\r\n1000110\r\n1011000\r\n1100000\r\n1011000", 'К');
            ABC.Add("0001000\r\n0010100\r\n0100010\r\n0100010\r\n1000001", 'Л');
            ABC.Add("1000001\r\n1100011\r\n1010101\r\n1001001\r\n1000001", 'М');
            ABC.Add("1000001\r\n1000001\r\n1000001\r\n1111111\r\n1000001", 'Н');
            ABC.Add("0001000\r\n0010100\r\n0100010\r\n1100011\r\n0100010", 'О');
            ABC.Add("1111111\r\n1000001\r\n1000001\r\n1000001\r\n1000001", 'П');
            ABC.Add("1111111\r\n1000001\r\n1000001\r\n1111111\r\n1000000", 'Р');
            //ABC.Add("1111111\r\n1000000\r\n1000000\r\n1000000\r\n1000000", 'С');
            ABC.Add("1111111\r\n0001000\r\n0001000\r\n0001000\r\n0001000", 'Т');
            ABC.Add("1000001\r\n0100010\r\n0010100\r\n0001000\r\n0010000", 'У');
            ABC.Add("1111111\r\n1001001\r\n1001001\r\n1111111\r\n0001000", 'Ф');
            ABC.Add("1000001\r\n0100010\r\n0010100\r\n0001000\r\n0010100", 'Х');
            ABC.Add("1000010\r\n1000010\r\n1000010\r\n1000010\r\n1000010", 'Ц');
            ABC.Add("1000001\r\n1000001\r\n1000001\r\n1111111\r\n0000001", 'Ч');
            //ABC.Add("1001001\r\n1001001\r\n1001001\r\n1001001\r\n1001001", 'Ш');
            //ABC.Add("1001001\r\n1001001\r\n1001001\r\n1001001\r\n1001001", 'Щ');
            ABC.Add("1100000\r\n0100000\r\n0100000\r\n0111111\r\n0100001", 'Ъ');
            ABC.Add("1000001\r\n1000001\r\n1000001\r\n1111001\r\n1001001", 'Ы');
            ABC.Add("1000000\r\n1000000\r\n1000000\r\n1111111\r\n1000001", 'Ь');
            ABC.Add("0011110\r\n0100001\r\n1000001\r\n0001110\r\n1000001", 'Э');
            ABC.Add("1011111\r\n1010001\r\n1010001\r\n1110001\r\n1010001", 'Ю');
            ABC.Add("1111111\r\n1000001\r\n1000001\r\n1111111\r\n0000111", 'Я');
        }

        /// <summary>
        /// Копирует даные из сортрованного словаря в список проверяя на уникальность
        /// </summary>
        private static List<string> DictionaryToList(SortedDictionary<int, List<Word>> words)
        {
            var stringWords = new List<string>();

            foreach (KeyValuePair<int, List<Word>> pair in words)
            {
                foreach (Word word in pair.Value)
                {
                    if (!stringWords.Contains(word.value))
                    {
                        stringWords.Add(word.value);
                    }
                }
            }

            return stringWords;
        }

        private static void WriteStatistics(GameAPI api)
        {
            Console.WriteLine("Сделано ходов - " + api.GetStatistics().moves);
            Console.WriteLine("Найдено слов - " + api.GetStatistics().words);
            Console.WriteLine("Получено окочков - " + api.GetStatistics().points);
        }


        static void Main(string[] args)
        {
            //передаем в экземпляр игрового API  аргументы командной строки с адресом и ключом
            var api = new GameAPI(args[0], args[1]);

            var ABC = new Dictionary<String, char>();
            InitABC(ABC);

            var visibleArea = api.StartGame();
            var searcher = new WordSearcher(visibleArea, api, ABC);

            var words = searcher.searchWords();

            var stringWords = DictionaryToList(words);

            foreach (string word in stringWords)
            {
                Console.WriteLine(word);
            }

            var serializedWords = JsonConvert.SerializeObject(stringWords);
            api.SendWords(serializedWords);

            WriteStatistics(api);

            api.EndGame();

            //Console.ReadKey();
        }
    }
}

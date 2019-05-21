using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientWordSearcher
{
    class WordSearcher
    {
        const int CHARACTER_HEIGHT = 7;
        const int CHARACTER_LENGTH = 7;

        //координаты углов одного тайла карты
        private Coordinates leftUpCorner = new Coordinates(0, 0);
        private Coordinates rightDownCorner = new Coordinates(0, 0);
        private Coordinates leftDownCorner = new Coordinates(0, 0);
        private Coordinates rightUpCorner = new Coordinates(0, 0);

        private int mapLength = 0;
        private int mapHeight = 0;
        //координаты левого верхнего угла видимой области относительно карты
        private Coordinates locationCurrentVisibleArea = new Coordinates(0, 0);

        //текущая видимая площадка
        private Area currentVisibleArea = null;

        private GameAPI api = null;

        //алфавит с образцами букв
        private Dictionary<String, char> ABC;

        //конструктор инициализуерт новый экземпляр
        public WordSearcher(Area visibleArea, GameAPI gameAPI, Dictionary<String, char> characterSamples)
        {
            api = gameAPI;
            currentVisibleArea = visibleArea;
            ABC = characterSamples;
        }

        /// <summary>
        /// Выполняет движение в заданном направлении на 
        /// заданное кол-во шагов и запоминает текущие координаты
        /// </summary>
        /// <param name="direction">Направление движения</param>
        /// <param name="stepCount">Число ходов в заданном направлении</param>
        private void Move(string direction, int stepCount)
        {
            Area area = null;
            if (stepCount > 0)
            {
                for (var i = 0; i != stepCount; i++)
                {
                    area = api.Move(direction);

                    if (direction == "right")
                        locationCurrentVisibleArea.x++;

                    if (direction == "left")
                        locationCurrentVisibleArea.x--;

                    if (direction == "up")
                        locationCurrentVisibleArea.y++;

                    if (direction == "down")
                        locationCurrentVisibleArea.y--;

                }
                currentVisibleArea = area;

            }
            else
            {
                if (stepCount != 0)
                    Console.WriteLine("Число шагов не может быть отрицательным");
            }
        }

        /// <summary>
        /// Передвигает видимую область к заданной клетке, 
        /// использует метод Move в своей работе
        /// </summary>
        /// <param name="targetCell">Координаты клетки</param>
        private void MoveToCell(Coordinates targetCell)
        {
            var xStep = targetCell.x - locationCurrentVisibleArea.x;
            var yStep = targetCell.y - locationCurrentVisibleArea.y;

            if (xStep < 0)
            {
                Move("left", -1 * xStep);
            }
            else
            {
                Move("right", xStep);
            }

            if (yStep < 0)
            {
                Move("down", -1 * yStep);
            }
            else
            {
                Move("up", yStep);
            }
        }

        /// <summary>
        /// Если в строке представляющей площадку есть какая-либо буква, возвращает ее, иначе - '?'
        /// </summary>
        /// <param name="stringArea">Строковое представление площадки</param>
        /// <returns></returns>
        private char RecognizeCharacter(string stringArea)
        {
            if (ABC.ContainsKey(stringArea))
            {
                return ABC[stringArea];
            }
            return '?';
        }
        /// <summary>
        /// Метод обходит область 13х13 и копирует полученные данные в массив 
        /// </summary>
        /// <param name="studyArea"></param>
        private void MoveEndCopyTo13x13Area(Area studyArea)
        {
            var leftUpCornerCopiedArea = new Coordinates(2, 0);
            currentVisibleArea.SelfCopy(studyArea, leftUpCornerCopiedArea);

            Move("down", currentVisibleArea.height);
            leftUpCornerCopiedArea = new Coordinates(2, 5);
            currentVisibleArea.SelfCopy(studyArea, leftUpCornerCopiedArea);

            Move("down", 3);
            leftUpCornerCopiedArea = new Coordinates(2, 8);
            currentVisibleArea.SelfCopy(studyArea, leftUpCornerCopiedArea);

            Move("left", 2);
            leftUpCornerCopiedArea = new Coordinates(0, 8);
            currentVisibleArea.SelfCopy(studyArea, leftUpCornerCopiedArea);

            Move("up", 5);
            leftUpCornerCopiedArea = new Coordinates(0, 3);
            currentVisibleArea.SelfCopy(studyArea, leftUpCornerCopiedArea);

            Move("up", 3);
            leftUpCornerCopiedArea = new Coordinates(0, 0);
            currentVisibleArea.SelfCopy(studyArea, leftUpCornerCopiedArea);

        }
        /// <summary>
        /// Определяет координату верхней горизонтальной границы слова
        /// с помощью буквы, которая встретилась
        /// </summary>
        /// <returns></returns>
        private int findUpBorder()
        {
            //просматривается площадка 13х13 в центре которой находится клетка с '1' 
            var studyArea = new Area(2 * CHARACTER_LENGTH - 1, 2 * CHARACTER_HEIGHT - 1);
            var oneCoord = currentVisibleArea.GetCellCoordWithOne();

            //передвигаем видмую область к месту начала просмотра
            var target = new Coordinates(locationCurrentVisibleArea.x + oneCoord.x - (currentVisibleArea.length - (CHARACTER_LENGTH)), locationCurrentVisibleArea.y - oneCoord.y + CHARACTER_HEIGHT - 1);
            MoveToCell(target);

            MoveEndCopyTo13x13Area(studyArea);

            //после получения всех данных проверяет по алфавиту есть ли где-то в области буква,
            //если есть запоминает координату верхней ее границы
            int coordBorder = 0;
            for (var i = 0; i != studyArea.height - CHARACTER_HEIGHT + 1; i++)
            {
                for (var j = 0; j != studyArea.length - CHARACTER_LENGTH + 1; j++)
                {
                    //буква может  находиться в области 7х5
                    var leftUpCorner = new Coordinates(j, i);
                    var rightDownCornerX5 = new Coordinates(j + CHARACTER_LENGTH, i + CHARACTER_HEIGHT - 2);
                    //и 7x7
                    var rightDownCornerX7 = new Coordinates(j + CHARACTER_LENGTH, i + CHARACTER_HEIGHT);

                    //перевод массивов в соответствующие им строки
                    var stringArray7X5 = Area.ArrayToString(leftUpCorner, rightDownCornerX5, studyArea);
                    var stringArray7X7 = Area.ArrayToString(leftUpCorner, rightDownCornerX7, studyArea);

                    if (ABC.ContainsKey(stringArray7X5) || (ABC.ContainsKey(stringArray7X7)))
                    {
                        coordBorder = locationCurrentVisibleArea.y - i;
                        //передвигаем область к найденной границе
                        var finishTarget = new Coordinates(locationCurrentVisibleArea.x + j, coordBorder);
                        MoveToCell(finishTarget);

                        return coordBorder;
                    }
                }

            }
            return coordBorder;
        }

        /// <summary>
        /// Определяет координату левой границы слова
        /// </summary>
        /// <returns></returns>
        private int findLeftBorder()
        {
            //область 11x7 для предполагаемой буквы
            var studyArea = new Area(currentVisibleArea.length, CHARACTER_HEIGHT);
            //идет влево до тех пор пока не встретит область не содержащую буквы
            var characterIsSearched = true;
            while (characterIsSearched)
            {
                Move("left", CHARACTER_LENGTH + 1);

                //5 строк достаточно, чтобы определить большинство букв
                var stringStudyArea = Area.ArrayToString(new Coordinates(0, 0), new Coordinates(CHARACTER_LENGTH, CHARACTER_HEIGHT - 2), currentVisibleArea);

                //если в алфавите данному образцу соответсвует какая либо буква, двигаемся дальше 
                if (ABC.ContainsKey(stringStudyArea))
                    continue;

                else   //если буквы не нашлось то сканируем область 7х7, возможно это одна из букв определяемых только всеми 7 строками
                {
                    currentVisibleArea.SelfCopy(studyArea, new Coordinates(0, 0));
                    Move("down", 2);
                    currentVisibleArea.SelfCopy(studyArea, new Coordinates(0, 2));
                    Move("up", 2);

                    stringStudyArea = Area.ArrayToString(new Coordinates(0, 0), new Coordinates(CHARACTER_LENGTH, CHARACTER_HEIGHT), studyArea);

                    //если этой строки нет в алфавите, то это начало слова 
                    if (!ABC.ContainsKey(stringStudyArea))
                        characterIsSearched = false;

                }
            }

            return locationCurrentVisibleArea.x + CHARACTER_LENGTH + 1;
        }

        /// <summary>
        /// Распознает найденное слово, возвращает экземпляр этого слова
        /// </summary>
        /// <returns></returns>
        private Word RecognizeWord()
        {
            //запоминаем место в котором нашли слово
            var currentLocation = new Coordinates(0, 0);
            currentLocation.x = locationCurrentVisibleArea.x;
            currentLocation.y = locationCurrentVisibleArea.y;

            //структура для найденного слова, в конце содержит распознанное слово
            Word word;
            word.value = "";

            word.location = new Coordinates(0, 0);
            word.location.y = findUpBorder();
            word.location.x = findLeftBorder();

            //для букв однозначно определяемых только 7 строками
            var studyArea = new Area(currentVisibleArea.length, CHARACTER_HEIGHT);

            //пока есть буквы в конце слова двигается вправо и добавляет найденные буквы к word.value
            var characterIsSearched = true;
            while (characterIsSearched)
            {
                Move("right", CHARACTER_LENGTH + 1);

                var stringStudyArea = Area.ArrayToString(new Coordinates(0, 0), new Coordinates(CHARACTER_LENGTH, CHARACTER_HEIGHT - 2), currentVisibleArea);

                if (ABC.ContainsKey(stringStudyArea))
                {
                    word.value += RecognizeCharacter(stringStudyArea);
                }
                else
                {
                    currentVisibleArea.SelfCopy(studyArea, new Coordinates(0, 0));
                    Move("down", 2);
                    currentVisibleArea.SelfCopy(studyArea, new Coordinates(0, 2));
                    Move("up", 2);

                    stringStudyArea = Area.ArrayToString(new Coordinates(0, 0), new Coordinates(CHARACTER_LENGTH, CHARACTER_HEIGHT), studyArea);

                    if (ABC.ContainsKey(stringStudyArea))
                    {
                        word.value += RecognizeCharacter(stringStudyArea);
                    }
                    else
                    {
                        characterIsSearched = false;
                    }
                }

            }

            //передвигаемся место старта
            MoveToCell(new Coordinates(currentLocation.x, currentLocation.y));

            return word;
        }

        /// <summary>
        /// Двигаясь по одной из сторон спирали проверяет нет ли на ней '1'
        /// </summary>
        /// <param name="direction">Задает направление для движения</param>
        /// <param name="stepsCount">количество ходов</param>
        /// <returns></returns>
        private bool SearchInSideOfGyrate(string direction, int stepsCount)
        {
            //на каждом шаге проверяем нет ли единицы
            for (var j = 0; j != stepsCount; j++)
            {
                Move(direction, 1);

                if (currentVisibleArea.GetCellCoordWithOne() != null)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Ищет первое слово двигаясь по расширяющейся спирали, как только встретился '1', останавливается
        /// </summary>
        private void SearchFirstWord()
        {
            var firstWordIsSearched = false;

            var visibleAreaCountOnSide = 1;

            while (!firstWordIsSearched)
            {
                if (!firstWordIsSearched)
                    firstWordIsSearched = SearchInSideOfGyrate("up", (currentVisibleArea.height + CHARACTER_HEIGHT - 1) * visibleAreaCountOnSide);
                if (!firstWordIsSearched)
                    firstWordIsSearched = SearchInSideOfGyrate("right", (currentVisibleArea.length + CHARACTER_LENGTH * 2) * visibleAreaCountOnSide);

                visibleAreaCountOnSide++;

                if (!firstWordIsSearched)
                    firstWordIsSearched = SearchInSideOfGyrate("down", (currentVisibleArea.height + CHARACTER_HEIGHT - 1) * visibleAreaCountOnSide);
                if (!firstWordIsSearched)
                    firstWordIsSearched = SearchInSideOfGyrate("left", (currentVisibleArea.length + CHARACTER_LENGTH * 2) * visibleAreaCountOnSide);

                visibleAreaCountOnSide++;
            }
        }

        /// <summary>
        /// Добавляет слово в сортированный по ключу словарь
        /// </summary>
        /// <param name="word">Добавляемое слово</param>
        /// <param name="searchedWords">Словарь в который вставляется слово</param>
        private void AddWordIntoSearchedWords(Word word, SortedDictionary<int, List<Word>> searchedWords)
        {
            if (!searchedWords.ContainsKey(word.value.Length))
                searchedWords.Add(word.value.Length, new List<Word>());

            searchedWords[word.value.Length].Add(word);
        }

        /// <summary>
        /// Проверяет не содержится ли клетка в слове, учитывает
        /// слова в соседних тайлах
        /// </summary>
        /// <param name="word">Слово, принадлежность которому проверяется</param>
        /// <param name="cell">Проверяемая клетка</param>
        /// <returns></returns>
        private bool WordContainsCell(Word word, Coordinates cell)
        {
            var cellInCentrWord = (cell.x >= word.location.x) && (cell.y <= word.location.y) && (cell.x <= word.location.x + word.value.Length * (CHARACTER_LENGTH + 1)) && (cell.y >= word.location.y - CHARACTER_HEIGHT);
            var cellInWordOfRightTile = (cell.x >= word.location.x + mapLength) && (cell.y <= word.location.y) && (cell.x <= word.location.x + word.value.Length * (CHARACTER_LENGTH + 1) - 1 + mapLength) && (cell.y >= word.location.y - CHARACTER_HEIGHT);
            var cellInWordOfLeftTile = (cell.x >= word.location.x - mapLength) && (cell.y <= word.location.y) && (cell.x <= word.location.x + word.value.Length * (CHARACTER_LENGTH + 1) - 1 - mapLength) && (cell.y >= word.location.y - CHARACTER_HEIGHT);
            var cellInWordOfUpTile = (cell.x >= word.location.x) && (cell.y <= word.location.y + mapHeight) && (cell.x <= word.location.x + word.value.Length * (CHARACTER_LENGTH + 1) - 1) && (cell.y >= word.location.y - CHARACTER_HEIGHT + mapHeight);
            var cellInWordOfDownTile = (cell.x >= word.location.x) && (cell.y <= word.location.y - mapHeight) && (cell.x <= word.location.x + word.value.Length * (CHARACTER_LENGTH + 1) - 1) && (cell.y >= word.location.y - CHARACTER_HEIGHT - mapHeight);

            return cellInCentrWord||cellInWordOfRightTile||cellInWordOfLeftTile||cellInWordOfUpTile||cellInWordOfDownTile;

        }

        /// <summary>
        /// Проверяет не принадлежит ли клетка какому либо слову из уже найденных.
        /// </summary>
        /// <param name="cell">Проверяемая клетка</param>
        /// <param name="searchedWords">Словарь с найденными словами</param>
        /// <returns></returns>
        private bool CellInWord(Coordinates cell, SortedDictionary<int, List<Word>> searchedWords)
        {
            foreach (KeyValuePair<int, List<Word>> list in searchedWords)
            {
                foreach (Word word in list.Value)
                {
                    if (WordContainsCell(word, cell))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет не было ли слово уже найдено с учетом его координат
        /// </summary>
        /// <param name="word">Слово, наличие которог проверяется</param>
        /// <param name="searchedWords">Словарь с найденными словами</param>
        /// <returns></returns>
        private bool WordIsFinded(Word word, SortedDictionary<int, List<Word>> searchedWords)
        {
            foreach (KeyValuePair<int, List<Word>> list in searchedWords)
            {
                if (list.Value.Contains(word))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет не было ли найдено данное слово без учета координат,
        /// т.е. проверяет только по значению слова, если найдено возвращает его
        /// </summary>
        /// <param name="word">Слово, наличие которого проверяется</param>
        /// <param name="searchedWords">Словарь с найденными словами</param>
        /// <returns></returns>
        private Word WordIsFinded(string word, SortedDictionary<int, List<Word>> searchedWords)
        {
            foreach (KeyValuePair<int, List<Word>> list in searchedWords)
            {
                foreach (Word wordIterator in list.Value)
                {
                    if (word == wordIterator.value)
                        return wordIterator;
                }
            }
            Word nullWord;
            nullWord.location = null;
            nullWord.value = null;
            return nullWord;
        }

        /// <summary>
        /// Проверяет находится ли видимая площадка в пределах одного тайла
        /// </summary>
        /// <param name="location">Координаты площадки</param>
        /// <returns></returns>
        private bool VisibleAreaInMap(Coordinates location)
        {
            if ((location.y <= leftUpCorner.y) && (location.y >= leftDownCorner.y))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// После нахождения первого слова двигается вниз до тех пор пока снова не встретит первое слово,
        /// по пути распознает все попавшиеся слова
        /// </summary>
        /// <param name="firstWord">Экземпляр первого слова</param>
        /// <param name="searchedWords">Коллекция найденных слов</param>
        private void MoveToDownAndRecognizeWords(Word firstWord, SortedDictionary<int, List<Word>> searchedWords)
        {
            while (true)
            {
                while (currentVisibleArea.GetCellCoordWithOne() == null)
                    Move("down", 1);

                var cellWithOne = currentVisibleArea.GetCellCoordWithOne();

                //если координата ячейки с единицей принадлежит какому-либо найденному слову, то пропускаем ее
                if (CellInWord(new Coordinates(locationCurrentVisibleArea.x + cellWithOne.x, locationCurrentVisibleArea.y - cellWithOne.y), searchedWords))
                {
                    Move("down", 1);
                }
                else
                {
                    var word = RecognizeWord();
                    Console.WriteLine(word.value + "  " + api.GetStatistics().moves);

                    if (word.value == firstWord.value)
                    {
                        leftDownCorner = word.location;

                        mapHeight = leftUpCorner.y - leftDownCorner.y;

                        AddWordIntoSearchedWords(word, searchedWords);
                        break;
                    }
                    else
                    {
                        AddWordIntoSearchedWords(word, searchedWords);
                    }
                }
            }
        }

        /*метод выполняет поиск слов
        сначала находит первое слово путем движения по расширяющейся спирали
        затем двигается вниз проверяя по пути попавшиеся слова до тех пор пока 
        снова не встретит первое слово.
        Далее двигаясь змейкой вверх вниз проверяет тайл и так до тех пор пока снова не встретит первое слово 
        */
        public SortedDictionary<int, List<Word>> searchWords()
        {
            //коллекция для всех распознанных слов
            var searchedWords = new SortedDictionary<int, List<Word>>();

            SearchFirstWord();
            var firstWord = RecognizeWord();
            Console.WriteLine(firstWord.value + " " + api.GetStatistics().moves);
            AddWordIntoSearchedWords(firstWord, searchedWords);

            //передвигаемся в стартовую точку основного поиска
            MoveToCell(firstWord.location);

            leftUpCorner = firstWord.location;

            MoveToDownAndRecognizeWords(firstWord, searchedWords);

            var direction = "up";
            var mapLengthIsCalcuted = false;
            var firstWordIsFindedAgain = false;

            //двигается вправо змейкой (то вниз, то вверх) распознает все слова до тех пор пока не встретит первое слово
            while (!firstWordIsFindedAgain)
            {
                Move("right", (CHARACTER_LENGTH + 1) * 2 + currentVisibleArea.length);

                while (VisibleAreaInMap(locationCurrentVisibleArea) && (!firstWordIsFindedAgain))
                {
                    while ((currentVisibleArea.GetCellCoordWithOne() == null) && (VisibleAreaInMap(locationCurrentVisibleArea)))
                    {
                        Move(direction, 1);
                    }

                    var cellWithOne = currentVisibleArea.GetCellCoordWithOne();

                    //если дошли до края тайла, а единицы нет
                    if (cellWithOne == null)
                    {
                        break;
                    }

                    //если координата ячейки с единицей принадлежит какому-либо найденному слову, то пропускаем ее
                    if (CellInWord(new Coordinates(locationCurrentVisibleArea.x + cellWithOne.x, locationCurrentVisibleArea.y - cellWithOne.y), searchedWords))
                    {
                        if (mapLengthIsCalcuted)
                        {
                            if (WordContainsCell(firstWord, new Coordinates(locationCurrentVisibleArea.x + cellWithOne.x, locationCurrentVisibleArea.y - cellWithOne.y)))
                            {
                                firstWordIsFindedAgain = true;
                            }
                        }
                        Move(direction, 1);
                    }
                    else
                    {
                        var searchedWord = RecognizeWord();
                        Console.WriteLine(searchedWord.value + "  " + api.GetStatistics().moves);

                        if (searchedWord.value == firstWord.value)
                        {
                            firstWordIsFindedAgain = true;
                        }
                        else if (!mapLengthIsCalcuted)
                        {
                            //считаем длину тайла карты
                            var findedWord = WordIsFinded(searchedWord.value, searchedWords);
                            if ((findedWord.location != null) && (findedWord.value != null))
                            {
                                mapLength = searchedWord.location.x - findedWord.location.x;
                                AddWordIntoSearchedWords(searchedWord, searchedWords);
                                mapLengthIsCalcuted = true;
                            }
                            else
                            {
                                AddWordIntoSearchedWords(searchedWord, searchedWords);
                            }
                        }
                        else
                        {
                            AddWordIntoSearchedWords(searchedWord, searchedWords);
                        }

                    }

                }

                //если область прошла весь путь по вертикали, то разворачивает ее в обратную сторону
                if (locationCurrentVisibleArea.y > leftUpCorner.y)
                {
                    while (locationCurrentVisibleArea.y > leftUpCorner.y)
                        Move("down", 1);
                    direction = "down";
                }

                if (locationCurrentVisibleArea.y < leftDownCorner.y)
                {
                    while (locationCurrentVisibleArea.y < leftDownCorner.y)
                        Move("up", 1);
                    direction = "up";
                }
            }

            return searchedWords;
        }
    }
}


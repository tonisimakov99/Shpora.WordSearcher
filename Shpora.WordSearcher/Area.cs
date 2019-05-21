using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientWordSearcher
{
    class Area        //класс предсталяет тип для хранения двумерной площадки
    {
        //длина площадки
        private int areaLength;
        public int length
        {
            get 
            {
                return areaLength > 0 ? areaLength : -1;
            }
            private set
            {
                if (value > 0)
                    areaLength = value;
            }
        }

        //высота площадки
        private int areaHeight;
        public int height
        {
            get
            {
                return areaHeight > 0 ? areaHeight : -1;
            }
            private set
            {
                if (value > 0)
                    areaHeight = value;
            }
        }

        //матрица хранящая значения
        private char[,] valuesArea;
        
        /// <summary>
        /// Получает ширину площадки по ее строковому представлению
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private int GetLength(string values)
        {
            //строка содержит значения матрицы записанные построчно, каждая строка площадки отделена от других спец.символами
            //символ возврата каретки сразу за концом первой строки площадки, его номер соответствует длине строки области
            var index = 0;
            while (values[index] != '\r')
            {
                index++;
            }
            return index;
        }

        /// <summary>
        /// Получает высоту площадки по ее строковому представления
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private int GetHeight(string values)
        {
            //в конце каждой строки области находятся 2 специальных символа
            const int specSymbolNumber = 2;

            var arrayLength = GetLength(values);

            /*в каждой строке площадки кроме последней, находятся управляющие символы для перевода строки
              чтобы узнать высоту видимой области достаточно количество всех символов без учета
              последней строки площадки поделить на ширину площадки + количество специальных символов 
            */
            var index = values.LastIndexOf('\n');
            var valuesLengthWithoutLastArrayString = index + 1;
            return (valuesLengthWithoutLastArrayString) / (arrayLength + specSymbolNumber) + 1;
        }

        /// <summary>
        /// Конвентирует строковое представление площадки в экземпляр Area
        /// </summary>
        /// <param name="values">Строка представляющая площадку, строчки площадки разделены символами переноса строки и возврата каретки</param>
        public void StringToArea(string values)
        {
            //в конце каждой строки области находятся 2 специальных символа
            const int specSymbolNumber = 2;

            //вычисляем ширину и высоту площадки
            length = GetLength(values);
            height = GetHeight(values);

            //выделяем память под массив соответсвющего размера
            valuesArea = new char[length, height];

            //переписываем символы из строки представляющей площадку в массив 
            var symbolIndex = 0;
            for (var heightIndex = 0; heightIndex < height; heightIndex++)         //для каждой строки
            {
                for (var lengthIndex = 0; lengthIndex < length; lengthIndex++)     //для каждого столбца
                {
                    if (symbolIndex < values.Length)
                    {
                        valuesArea[lengthIndex, heightIndex] = values[symbolIndex];
                        symbolIndex++;
                    }
                }
                symbolIndex += specSymbolNumber;  //учитываем специальные символы переводящие каретку на новую строку
            }
        }

        /// <summary>
        /// Инициализирует экземпляр площадки используя ее строковое представление
        /// </summary>
        /// <param name="values">Строковое представление площадки</param>
        public Area(string values)       
        {
            StringToArea(values);
        }

        /// <summary>
        /// Инициализирует экземпляр площадки, задет только размеры площадки, не заполняет ячейки
        /// </summary>
        /// <param name="length">Ширина площадки</param>
        /// <param name="height">Высота площадки</param>
        public Area(int length, int height)
        {
            areaLength = length;
            areaHeight = height;
            valuesArea = new char[length, height];
        }

        /// <summary>
        /// Возвращает значение из заданной ячейки
        /// </summary>
        /// <param name="cell">Координаты ячейки</param>
        /// <returns></returns>
        public char GetValueCell(Coordinates cell)
        {
            return valuesArea[cell.x, cell.y];
        }

        /// <summary>
        /// Возвращает значение из заданной ячейки
        /// </summary>
        /// <param name="x">Номер столбца</param>
        /// <param name="y">Номер строки</param>
        /// <returns></returns>
        public char GetValueCell(int x, int y)
        {
            return valuesArea[x, y];
        }

        /// <summary>
        /// Устанавливает заданное значение в соответствующую клетку
        /// </summary>
        /// <param name="cell">Координаты ячейки в которую устанавливаем значение</param>
        /// <param name="value">устанавливаемое значение</param>
        public void SetValueCell(Coordinates cell, char value)
        {

            valuesArea[cell.x, cell.y]=value;
        }

        /// <summary>
        /// Копирует себя в другую площадку 
        /// </summary>
        /// <param name="copiedArea">Площадка в которую копируется</param>
        /// <param name="leftUpCorner">Площадка скопируется правее и ниже этих координат включая их</param>
        public void SelfCopy(Area copiedArea, Coordinates leftUpCorner)
        {
            for (var i = 0; i != areaHeight; i++)
            {
                for(var j = 0; j != areaLength; j++)
                {
                    Coordinates cell = new Coordinates(leftUpCorner.x+j, leftUpCorner.y+i);
                    copiedArea.SetValueCell(cell, valuesArea[j,i]);
                }
            }
        }

        /// <summary>
        /// Выводит площадку на консоль
        /// </summary>
        public void WriteArea()
        {
            for (var i = 0; i != height; i++)
            {
                for (var j = 0; j != length; j++)
                {
                    Console.Write(valuesArea[j, i]);
                }
                Console.WriteLine();
            }
        }
        
        /// <summary>
        /// Конвентирует текущую площадку в строковое представление, затем возвращает ее
        /// </summary>
        /// <returns></returns>
        public string AreaToString()
        {
            var result = "";
            for (var i = 0; i != areaHeight; i++)
            {
                if (result != "")
                    result += "\r\n";

                for (var j = 0; j != areaLength; j++)
                {
                    result += valuesArea[j, i];
                }
            }
            return result;
        }

        /// <summary>
        /// Конвертирует выбранную область из переданного экземпляря площадки в строковое представление,
        /// затем возвращает соответствующую строку
        /// </summary>
        /// <param name="leftUpCorner">Левый верхний угол начиная с которого конвертируется площадка</param>
        /// <param name="rightDownCorner">Правый нижний угол до которого конвертируется</param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static string ArrayToString(Coordinates leftUpCorner, Coordinates rightDownCorner, Area area)
        {
            string result = "";
            for (int i = leftUpCorner.y; i != rightDownCorner.y; i++)
            {
                //В начале строки нет сисволов перевода строки и возврата каретки
                if (result != "")                  
                    result += "\r\n";

                for (int j = leftUpCorner.x; j != rightDownCorner.x; j++)
                {
                    result += area.GetValueCell(j, i);
                }
            }
            return result;
        }

        /// <summary>
        /// Возвращает ячейку содержащую '1', если таковая есть, иначе возвращает null
        /// </summary>
        /// <returns></returns>
        public Coordinates GetCellCoordWithOne()
        {
            for (var i = 0; i != areaHeight; i++)
            {
                for (var j = 0; j != areaLength; j++)
                {
                    if (valuesArea[j,i] == '1')
                    {
                        return new Coordinates(j, i);
                    }
                }
            }
            return null;
        }
    }
}

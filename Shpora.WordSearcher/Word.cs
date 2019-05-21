using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientWordSearcher
{
    struct Word
    {
        /// <summary>
        /// Строковое представление строки
        /// </summary>
        public string value;

        /// <summary>
        /// Координаты левой верхней клетки принадлежащей слову 
        /// </summary>
        public Coordinates location;

    }

}

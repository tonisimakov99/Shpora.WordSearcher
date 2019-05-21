using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientWordSearcher
{
    /// <summary>
    /// Предоставляет тип для хранения координат
    /// </summary>
    class Coordinates  
    {
        public int x=0;
        public int y=0;

        public Coordinates(int xCoord, int yCoord)
        {
            x = xCoord;
            y = yCoord;
        }
    }
}

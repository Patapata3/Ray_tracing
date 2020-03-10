using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class Chessboard : IPattern
    {
        /*Цвета клеток*/
        private Vector color1;
        private Vector color2;

        public Chessboard(Vector color1, Vector color2)
        {
            this.color1 = color1;
            this.color2 = color2;
        }

        public Vector ColorFromPoint(Point spot)
        {
            /* Проверка чётности и нечётности координат*/
            if (((int)(0.5 * spot.X + 1000) % 2 == 0 && (int)(0.5 * spot.Y + 1000) % 2 != 0) ||
                    ((int)(0.5 * spot.X + 1000) % 2 != 0 && (int)(0.5 * spot.Y + 1000) % 2 == 0))
            {

                return color1;
            }
            else
            {
                return color2;
            }
        }
    }
}

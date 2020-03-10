using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public interface IShape
    {
        Vector NormalFromPoint(Point spot);
        bool TryIntersect(Ray ray, out double dist, out Point spot); // Поиск точки пересечения луча и фигуры и расстояния до нее
        Point Translate(Point spot); // Перевод координат точки в систему координат на поверхности фигуры
    }
}

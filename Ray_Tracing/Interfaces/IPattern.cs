using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public interface IPattern
    {
        Vector ColorFromPoint(Point spot); // Определение цвета точки на поверхности объекта
    }
}

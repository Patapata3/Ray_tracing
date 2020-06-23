using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class Light
    {
        public Point Position { get; }
        public double Intensity { get; }

        public Light(Point pos, double intens)
        {
            Position = pos;
            Intensity = intens;
        }
    }
}

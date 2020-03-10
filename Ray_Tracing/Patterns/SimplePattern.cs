using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class SimplePattern : IPattern
    {
        private Vector diffuseColor;

        public SimplePattern(Vector color)
        {
            diffuseColor = color;
        }

        public Vector ColorFromPoint(Point spot)
        {
            return diffuseColor;
        }
    }
}

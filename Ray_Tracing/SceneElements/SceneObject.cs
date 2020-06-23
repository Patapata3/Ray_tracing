using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class SceneObject
    {
        public IShape Shape { get; private set; }
        public Material Material { get; private set; }

        public SceneObject(IShape shape, Material material)
        {
            Shape = shape;
            Material = material;
        }
    }
}

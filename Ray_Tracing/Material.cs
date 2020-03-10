using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Ray_Tracing
{
    public class Material
    {
        public IPattern Pattern { get; private set; }
        public double[] Albedo { get; private set; } // Коэффициенты возврата различных форм света
        public double SpecularExponent { get; private set; }
        public double RefractiveIndex { get; private set; }

        public Material() { }

        public Material (IPattern pattern, double exponent, double index, double[] albedo)
        {
            Pattern = pattern;
            SpecularExponent = exponent;
            Albedo = albedo;
            RefractiveIndex = index;
        }
    }
}

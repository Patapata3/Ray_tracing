using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Ray_Tracing
{
    public static class ColorHandler
    {
        /*Вычисление окончательного цвета, в который нужно закрасить точку*/
        public static Vector Calculate(Vector illuminationColor, Vector color, Vector reflectColor, Vector refractColor, double diffuseIntensity, double specularIntensity, Material material)
        {
            Vector result = Mult(illuminationColor, color) * diffuseIntensity * material.Albedo[0] + 
                (new Vector(1, 1, 1) * specularIntensity) * material.Albedo[1] + reflectColor * material.Albedo[2] +
                refractColor * material.Albedo[3];
            return result;
        }

        public static Color VectorToColor(Vector vector)
        {
            int R = (int)(255 * Math.Max(0, Math.Min(1, vector.X)));
            int G = (int)(255 * Math.Max(0, Math.Min(1, vector.Y)));
            int B = (int)(255 * Math.Max(0, Math.Min(1, vector.Z)));
            return Color.FromArgb(R, G, B);
        }

        /*Снижение насыщенности цвета для предотвращения засветки*/
        public static void NormalizeColor(ref Vector color)
        {
            double max = Math.Max(color.X, Math.Max(color.Y, color.Z));
            if (max > 1)
            {
                color = color / max;
            }
        }

        /*Покоординатное перемножение векторов цвета*/
        private static Vector Mult(Vector first, Vector second)
        {
            return new Vector(first.X * second.X, first.Y * second.Y, first.Z * second.Z);
        }

    }
}

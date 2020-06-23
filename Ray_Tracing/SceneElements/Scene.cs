using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Ray_Tracing
{
    public class Scene
    {
        private readonly List<SceneObject> objects;
        private readonly double field = Math.PI / 3;
        private readonly List<Light> lights;
        private readonly int maxDepth = 4;

        public Scene(List<SceneObject> shapes, List<Light> lights)
        {
            objects = shapes;
            this.lights = lights;
        }

        public void Draw(int startWidth, int endWidth, int startHeight, int endHeight, int height, int width, ref byte[] rgbValues)
        {
            for (int j = startHeight; j < endHeight; j++)
            {
                for (int i = startWidth; i < endWidth; i++)
                {
                    double x = (2 * (i + 0.5) / width - 1) * Math.Tan(field/2) * width / height;
                    double y = -(2 * (j + 0.5) / height - 1) * Math.Tan(field / 2);
                    Ray beam = new Ray(new Point(0, 0, 0), new Point(x, y, 1));
                    SetPixel(i, j, ref rgbValues, width, ColorHandler.VectorToColor(TraceRay(beam, 0)));
                }
               
            }
        }

        private Vector TraceRay(Ray beam, int depth)
        {
            Vector result = new Vector(0.2, 0.7, 0.8);    
            double diffuseIntensity = 0;
            double specularIntensity = 0;
            Material material = new Material();
            Vector normal = new Vector(0, 0, 0);
            Point litSpot = new Point(0, 0, 0);

            /*Возвращаем фоновый цвет, если на пути луча нет объекта*/
            if(depth >= maxDepth || !ShapeMet(beam, ref litSpot, ref normal, ref material, out var shape))
            {
                return result;
            }
            else
            {
                result = material.Pattern.ColorFromPoint(shape.Translate(litSpot));
            }

            Material tmpMaterial = new Material();
            Vector shadowNormal = new Vector(0, 0, 0);
            Vector reflectColor = new Vector(0, 0, 0);
            if (material.Albedo[2] != 0) //Проверка объекта на зеркальность
            {
                Point reflectSpot = new Point(litSpot.X, litSpot.Y, litSpot.Z);
                Vector reflectDir = Reflect(beam.Direction, normal);
                /*Сдвиг освещенной точки вдоль нормали, чтобы избежать повторного пересечения с тем же объектом*/
                Offset(reflectSpot, reflectDir, normal);
                Ray reflect = new Ray(reflectSpot, reflectDir);
                reflectColor = TraceRay(reflect, depth + 1);
            }

            Vector refractColor = new Vector(0, 0, 0);
            if (material.Albedo[3] != 0) //Проверка объекта на прозрачность
            {
                Vector refractDir = Refract(beam.Direction, normal, material.RefractiveIndex);
                if (refractDir.Length != 0)
                {
                    Point refractSpot = new Point(litSpot.X, litSpot.Y, litSpot.Z);
                    /*Сдвиг освещенной точки вдоль нормали, чтобы избежать повторного пересечения с тем же объектом*/
                    Offset(refractSpot, refractDir, normal);
                    Ray refract = new Ray(refractSpot, refractDir);
                    refractColor = TraceRay(refract, depth + 1);
                }
            }

            Vector illuminationColor = new Vector(0, 0, 0);
            foreach (var light in lights)
            {
                Vector lightColor = new Vector(1, 1, 1);
                double intensity = light.Intensity;
                Vector lightDir = new Vector(litSpot, light.Position).Normalize();
                Point shadowSpot = new Point(litSpot.X, litSpot.Y, litSpot.Z);
                Offset(shadowSpot, lightDir, normal); // Сдвиг освещенной точки вдоль нормали, чтобы избежать повторного пересечения с тем же объектом
                Ray shadowRay = new Ray(shadowSpot, lightDir);
                /*Вычисление теней*/
                if (ShapeMet(shadowRay, ref shadowSpot, ref shadowNormal, ref tmpMaterial, out var shadowShape))
                {
                    if (tmpMaterial.Albedo[3] == 0) // Проверка объекта но прозрачность
                    {
                        continue;
                    }
                    intensity *= tmpMaterial.Albedo[3]; // Приглушение света в соответствии с прозрачностью объекта
                    if (tmpMaterial.Albedo[0] != 0) // Придание лучу цвета прозрачного объекта
                    {
                        lightColor = tmpMaterial.Pattern.ColorFromPoint(shadowSpot) * tmpMaterial.Albedo[3];
                    }
                }
                else
                {
                    specularIntensity += Math.Pow(Math.Max(0, Reflect(lightDir, normal) * beam.Direction), // Вычисление интенсивности бликов
                    material.SpecularExponent) * light.Intensity;
                }
                illuminationColor += lightColor; // Суммарный цвет освещения в точке
                diffuseIntensity += Math.Max(0, intensity * (lightDir * normal));
                
            }
            ColorHandler.NormalizeColor(ref illuminationColor); // Приведение цвета освещения к стандартному виду
            result = ColorHandler.Calculate(illuminationColor, result, reflectColor, 
                refractColor, diffuseIntensity, specularIntensity, material);
            ColorHandler.NormalizeColor(ref result); // Предотвращение засветки
            return result;
        }

        private bool ShapeMet(Ray beam, ref Point litspot, ref Vector normal, ref Material material, out IShape shape)
        {
            double closest = double.MaxValue;
            shape = null;
            
            foreach (var sceneObject in objects)
            {
                if (sceneObject.Shape.TryIntersect(beam, out double dist, out Point spot))
                {
                    // Проверка, является ли встреченный объект ближайшим
                    if (dist < closest)
                    {
                        closest = dist;
                        litspot = spot;
                        normal = sceneObject.Shape.NormalFromPoint(spot);
                        material = sceneObject.Material;
                        shape = sceneObject.Shape;
                    }
                }
            }
            return closest < 100; // Ограничение дальности видимости - 100
        }

        /*Вычисление отражённого луча*/
        private Vector Reflect(Vector lightDir, Vector normal)
        {
            Vector result = lightDir - normal * 2 * (lightDir * normal);
            return result;
        }

        /*Вычисление преломлённого луча*/
        private Vector Refract(Vector lightDir, Vector normal, double refractiveIndex)
        {
            double cos1 = -Math.Max(-1, Math.Min(1, lightDir * normal)); // Косинус угла падения луча
            double refr1 = 1; // Показатель преломления воздуха
            double refr2 = refractiveIndex; // Показатель преломления среды внутри объекта
            Vector n = new Vector(normal.X, normal.Y, normal.Z);
            if(cos1 < 0) // Если луч выходит из объекта, меняем местами показатели преомления
            {
                double temp = refr1;
                refr1 = refr2;
                refr2 = temp;
                cos1 *= -1;
                n = -normal;
            }
            double refr = refr1 / refr2;
            double k = 1 - refr * refr * (1 - cos1 * cos1); // Квадрат косинуса угла преломлённого луча
            if (k < 0) // Если такого угла не существует - полное отражение
            {
                return new Vector(0, 0, 0);
            }
            else
            {
                return lightDir * refr + n * (refr * cos1 - Math.Sqrt(k)); // Направление преломлённого луча
            }
        }

        private void Offset(Point spot, Vector lightDir, Vector normal)
        {
            if (lightDir * normal < 0) // Сдвиг освещенной точки вдоль нормали, чтобы избежать повторного пересечения с тем же объектом
            {
                spot.Translate(-normal.X * 0.001, -normal.Y * 0.001, -normal.Z * 0.001);
            }
            else
            {
                spot.Translate(normal.X * 0.001, normal.Y * 0.001, normal.Z * 0.001);
            }
        }

        private void SetPixel(int x, int y, ref byte[] rgbValues, int width, Color color)
        {
            int i = ((y * width) + x) * 4;

            rgbValues[i] = color.B;
            rgbValues[i + 1] = color.G;
            rgbValues[i + 2] = color.R;
            rgbValues[i + 3] = color.A;
        }
    }
}

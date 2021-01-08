using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Ray_Tracing.TaskHandling;

namespace Ray_Tracing
{
    public partial class Form1 : Form
    {
        private Bitmap picture;
        private int height = 768;
        private int width = 1024;
        private Scene scene;
        
        public Form1()
        {
            List <SceneObject> objects= new List<SceneObject>();
            List<Light> lights = new List<Light>();
            Material rock = new Material(new SimplePattern(new Vector(0.4, 0.4, .3)), 50, 1, new double[] { 0.6, 0.3, 0.1, 0 } );
            Material redRubber = new Material(new SimplePattern(new Vector(0.3, 0.1, 0.1)), 10, 1, new double[] { 0.9, 0.1, 0, 0 });
            Material mirror = new Material(new SimplePattern(new Vector(0.5, 0.5, 0.5)), 1425, 1, new double[] { 0, 10, 0.8, 0 });
            Material glass = new Material(new SimplePattern(new Vector(0.1, 0.1, 0.8)), 125, 1.5, new double[] { 0.6, 0.5, 0.1, 0.8 });
            Material chessboard = new Material(new Chessboard(new Vector(1, 1, 1), new Vector(0, 0, 0)), 50, 1, new double[] { 0.3, 0.3, 0, 0 });
            objects.Add(new SceneObject(new Sphere(new Point(-4, 0, 16), 2), rock));
            objects.Add(new SceneObject(new Sphere(new Point(7, 5, 18), 4), mirror));
            objects.Add(new SceneObject(new Sphere(new Point(-1, -1.5, 12), 2), glass));
            objects.Add(new SceneObject(new Sphere(new Point(1.5, -0.5, 18), 3), redRubber));
            objects.Add(new SceneObject(new Sphere(new Point(-5.5, -0.5, 10), 1.5), chessboard));
            objects.Add(new SceneObject(new Plane(0, 1, 0, 5), chessboard));
            //objects.Add(new SceneObject(new Plane(0, 0, 1, -25), chessboard));
            lights.Add(new Light(new Point(-20, 20, -20), 1.5));

            lights.Add(new Light(new Point(30, 50, 25), 1.8));
            lights.Add(new Light(new Point(30, 20, -30), 1.7));


            scene = new Scene(objects, lights);
            InitializeComponent();
            CreateScene();
            ShowPicture();
        }

        private void CreateScene()
        {
            picture = new Bitmap(width, height);
            var pictureData = picture.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, picture.PixelFormat);
            int byteNum = Math.Abs(pictureData.Stride) * picture.Height;
            TaskManager manager = new TaskManager(byteNum);
            Marshal.Copy(pictureData.Scan0, manager.ResultBuffer, 0, byteNum);

            List<Task> taskList = manager.StartDrawingTasks(scene, height, width);
            foreach(var task in taskList)
            {
                task.Wait();
            }

            Marshal.Copy(manager.ResultBuffer, 0, pictureData.Scan0, byteNum);
            picture.UnlockBits(pictureData);
        }

        private void ShowPicture()
        {
            pictureBox1.ClientSize = new Size(width, height);
            pictureBox1.Image = (Image)picture;
        }
    }
}

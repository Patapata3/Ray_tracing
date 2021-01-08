using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Math;

namespace Ray_Tracing.TaskHandling
{
    public class TaskManager
    {
        private byte[] resultBuffer;

        private BlockDivision blocks = getBlocks();

        public byte[] ResultBuffer => resultBuffer;

        public TaskManager(int bufferSize)
        {
            resultBuffer = new byte[bufferSize];
        }

        public List<Task> StartDrawingTasks(Scene scene, int height, int width)
        {
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < blocks.Height; i++)
            {
                for (int j = 0; j < blocks.Width; j++)
                {
                    int startwidth = j * width / blocks.Width;
                    int endWidth = (j + 1) * width / blocks.Width;
                    int startHeight = i * height / blocks.Height;
                    int endHeight = (i + 1) * height / blocks.Height;
                    taskList.Add(Task.Factory.StartNew(() => scene.Draw(startwidth, endWidth, startHeight, endHeight, height, width, ref resultBuffer)));
                }
            }
            return taskList;
        }

        private static BlockDivision getBlocks()
        {
            int threadNum = (int)(1.5 * Environment.ProcessorCount);
            for (int i = (int)Ceiling(Sqrt(threadNum)); i > 0; i--)
            {
                if (threadNum % i == 0)
                    return new BlockDivision(i, threadNum / i);
            }
            return new BlockDivision(0, 0);
        }
    }
}

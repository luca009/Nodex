using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nodex.Classes.Threading
{
    public class ParallelTextureCalculator
    {
        public ITexture Texture { get; }
        public int ResolutionX { get; }
        public int ResolutionY { get; }
        public int TileSizeX { get; }
        public int TileSizeY { get; }
        public float Scale { get; }
        private Chunk[] chunks;
        private int chunksX;
        private int chunksY;

        public ParallelTextureCalculator(ITexture texture, int resolutionX, int resolutionY, int tileSizeX, int tileSizeY, float scale)
        {
            Texture = texture;
            ResolutionX = resolutionX;
            ResolutionY = resolutionY;
            TileSizeX = tileSizeX;
            TileSizeY = tileSizeY;
            Scale = scale;

            int resolutionXRemainder = resolutionX;
            int chunksX = 0;
            for (int i = 0; i < resolutionX; i += tileSizeX)
            {
                resolutionXRemainder -= tileSizeX;
                chunksX++;
            }
            if (resolutionXRemainder > 0)
                chunksX++;

            int resolutionYRemainder = resolutionY;
            int chunksY = 0;
            for (int i = 0; i < resolutionY; i += tileSizeY)
            {
                resolutionYRemainder -= tileSizeY;
                chunksY++;
            }
            if (resolutionYRemainder > 0)
                chunksY++;

            this.chunksX = chunksX;
            this.chunksY = chunksY;

            chunks = new Chunk[chunksX * chunksY];

            //if (resolutionXRemainder > 0 && resolutionYRemainder > 0)
            //{
            //    for (int i = 0; i < chunksX * chunksY; i++)
            //    {
            //        if (i == chunksX * chunksY - 1)
            //            chunks[i] = new Chunk(texture, resolutionX - (chunksX - 1) * tileSizeX, resolutionY - (chunksY - 1) * tileSizeY);
            //        else if (i % chunksX == 0 && i > 0)
            //            chunks[i] = new Chunk(texture, resolutionX - (chunksX - 1) * tileSizeX, tileSizeY);
            //        else if (i > chunksX * (chunksY - 1))
            //            chunks[i] = new Chunk(texture, tileSizeX, resolutionY - (chunksY - 1) * tileSizeY);
            //        else
            //            chunks[i] = new Chunk(texture, tileSizeX, tileSizeY);
            //    }
            //}
            //else if (resolutionXRemainder > 0)
            //{
            //    for (int i = 0; i < chunksX * chunksY; i++)
            //    {
            //        if (i % chunksX == 0 && i > 0)
            //            chunks[i] = new Chunk(texture, resolutionX - (chunksX - 1) * tileSizeX, tileSizeY);
            //        else
            //            chunks[i] = new Chunk(texture, tileSizeX, tileSizeY);
            //    }
            //}
            //else if (resolutionYRemainder > 0)
            //{
            //    for (int i = 0; i < chunksX * chunksY; i++)
            //    {
            //        if (i > chunksX * (chunksY - 1))
            //            chunks[i] = new Chunk(texture, tileSizeX, resolutionY - (chunksY - 1) * tileSizeY);
            //        else
            //            chunks[i] = new Chunk(texture, tileSizeX, tileSizeY);
            //    }
            //}
            //else
            //{
                for (int i = 0; i < chunksX * chunksY; i++)
                {
                    chunks[i] = new Chunk(texture, tileSizeX - 1, tileSizeY - 1);
                }
            //}
        }

        public Bitmap Calculate()
        {
            int temp = chunks.Length;
            List<Thread> threads = new List<Thread>();
            Bitmap[] bitmaps = new Bitmap[temp];

            for (int i = 0; i < temp; i++)
            {
                // / (chunksX * TileSizeX)
                int offsetY = (int)Math.Floor((float)i / chunksX) * TileSizeY;
                //int offsetX = (int)(Math.Ceiling((float)i / ResolutionX) * TileSizeX);
                int offsetX = i * TileSizeX % ResolutionX;
                int offsetYAddend = (int)Math.Floor((float)i / chunksX);

                //Safety precations because I managed to fuck this loop up hard
                if (i >= temp)
                    goto OutOfLoop;
                int iTemp = i;

                Thread thread = new Thread(() => { bitmaps[iTemp] = chunks[iTemp].Calculate(Scale, offsetX, offsetY); });
                threads.Add(thread);
                thread.Start();
                while (thread.ThreadState != ThreadState.Running)
                    Thread.Sleep(2); //Wait a bit because threads are dumb
                thread.IsBackground = true;
            }

OutOfLoop:

            foreach (Thread thread in threads)
            {
                thread.Join();
                Thread.Sleep(2); //Wait a bit because threads are dumb
            }

            foreach (var item in bitmaps)
            {
                while (item == null)
                    Thread.Sleep(4);
            }

            foreach (Thread thread in threads)
            {
                if (thread.IsAlive)
                {
                    
                    thread.Abort();
                }
            }

            Thread.Sleep(4);

            return MergeBitmaps(bitmaps);
        }

        private Bitmap MergeBitmaps(Bitmap[] bitmaps)
        {
            Bitmap returnBitmap = new Bitmap(ResolutionX, ResolutionY);
            using (Graphics gfx = Graphics.FromImage(returnBitmap))
            {
                int offsetX = 0;
                int offsetY = 0;
                foreach (Bitmap bitmap in bitmaps)
                {
                    gfx.DrawImageUnscaled(bitmap, new Point(offsetX, offsetY));
                    offsetX += TileSizeX;
                    if (offsetX > ResolutionX)
                    {
                        offsetX = 0;
                        offsetY += TileSizeY;
                    }
                }
            }

            return returnBitmap;
        }
    }
}

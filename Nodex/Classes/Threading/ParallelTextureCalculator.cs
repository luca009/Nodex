using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Nodex.Classes.TextureGenerators.Dimensions;

namespace Nodex.Classes.Threading
{
    public class ParallelTextureCalculator
    {
        public ITexture Texture { get; }
        public int ResolutionX { get; private set; }
        public int ResolutionY { get; private set; }
        public int TileSizeX { get; }
        public int TileSizeY { get; }
        public double Scale { get; }
        public Vector4 Vector { get; private set; }
        public Dimension Dimension { get; private set; }
        private List<List<Chunk>> chunks;
        private int chunksX;
        private int chunksY;
        private double previousZ;
        private double previousW;
        private Rectangle bounds;
        private Bitmap cachedBitmap;

        public ParallelTextureCalculator(ITexture texture, int tileSizeX, int tileSizeY, double scale, Vector4 vector, Size size, Dimension dimension)
        {
            Texture = texture;
            ResolutionX = ExtraMath.CeilingStep(size.Width, tileSizeX);
            ResolutionY = ExtraMath.CeilingStep(size.Height, tileSizeY);
            TileSizeX = tileSizeX;
            TileSizeY = tileSizeY;
            Scale = scale;
            Dimension = dimension;
            previousZ = vector.Z;
            previousW = vector.W;
            Vector = vector;
            bounds = new Rectangle(new Point(ExtraMath.FloorStep(vector.X, tileSizeX), ExtraMath.FloorStep(vector.Y, tileSizeY)), new Size(ResolutionX, ResolutionY));

            int resolutionXRemainder = ResolutionX;
            int chunksX = 0;
            for (int i = 0; i < ResolutionX; i += tileSizeX)
            {
                resolutionXRemainder -= tileSizeX;
                chunksX++;
            }
            if (resolutionXRemainder > 0)
                chunksX++;

            int resolutionYRemainder = ResolutionY;
            int chunksY = 0;
            for (int i = 0; i < ResolutionY; i += tileSizeY)
            {
                resolutionYRemainder -= tileSizeY;
                chunksY++;
            }
            if (resolutionYRemainder > 0)
                chunksY++;

            this.chunksX = chunksX;
            this.chunksY = chunksY;


            chunks = new List<List<Chunk>>();

            //bounds = new Rectangle(new Point((int)vector.X, (int)vector.Y), new Size(ResolutionX, ResolutionY));

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

            //}
        }

        public Bitmap SetDimensionAndRecalculate(Dimension dimension)
        {
            Dimension = dimension;
            return Calculate(Vector, bounds, true);
        }

        public Bitmap Calculate(Vector4 vector, Rectangle cropRectangle, bool forceRecalculate = false)
        {
            Vector = vector;

            if (forceRecalculate)
            {
                foreach (List<Chunk> list in chunks)
                {
                    foreach (Chunk chunk in list)
                    {
                        chunk.ClearBitmap();
                    }
                }
                goto ForceRecalculate;
            }

            switch (Dimension)
            {
                case Dimension.ThreeD:
                    if (!(vector.Z == previousZ))
                    {
                        foreach (List<Chunk> list in chunks)
                        {
                            foreach (Chunk chunk in list)
                            {
                                chunk.ClearBitmap();
                            }
                        }
                    }
                    break;
                case Dimension.FourD:
                    if (!(vector.Z == previousZ && vector.W == previousZ))
                    {
                        foreach (List<Chunk> list in chunks)
                        {
                            foreach (Chunk chunk in list)
                            {
                                chunk.ClearBitmap();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            

            switch (Dimension)
            {
                case Dimension.TwoD:
                    if (bounds.Contains(cropRectangle) && cachedBitmap != null)
                        return cachedBitmap;
                    break;
                case Dimension.ThreeD:
                    if (bounds.Contains(cropRectangle) && cachedBitmap != null && vector.Z == previousZ)
                        return cachedBitmap;
                    break;
                case Dimension.FourD:
                    if (bounds.Contains(cropRectangle) && cachedBitmap != null && vector.Z == previousZ && vector.W == previousW)
                        return cachedBitmap;
                    break;
                default:
                    break;
            }

        ForceRecalculate:

            previousZ = vector.Z;
            previousW = vector.W;
            ResolutionX = ExtraMath.CeilingStep(cropRectangle.Width + (cropRectangle.X % TileSizeX), TileSizeX);
            ResolutionY = ExtraMath.CeilingStep(cropRectangle.Height + (cropRectangle.Y % TileSizeY), TileSizeY);
            Debugger.AddValue("ResX: " + ResolutionX);
            Debugger.AddValue("ResY: " + ResolutionY);
            int boundsX = ExtraMath.FloorStep(cropRectangle.X, TileSizeX);
            int boundsY = ExtraMath.FloorStep(cropRectangle.Y, TileSizeY);
            bounds = new Rectangle(new Point(boundsX, boundsY), new Size(ResolutionX, ResolutionY));
            Debugger.AddValue(bounds.ToString());

            List<Thread> threads = new List<Thread>();

            for (int y = bounds.Y; y < bounds.Height + bounds.Y; y += TileSizeY)
            {
                int row = ExtraMath.FloorStep(y, TileSizeY) / TileSizeY;

                if (row >= chunks.Count)
                    chunks.Add(new List<Chunk>());

                for (int x = bounds.X; x < bounds.Width + bounds.X; x += TileSizeX)
                {
                    int column = ExtraMath.FloorStep(x, TileSizeX) / TileSizeX;

                    if (column == chunks[row].Count)
                        chunks[row].Add(new Chunk(Texture, TileSizeX, TileSizeY, new Vector2(x, y)));
                    else if (column > chunks[row].Count)
                    {
                        while (chunks[row].Count <= column)
                        {
                            chunks[row].Add(new Chunk(Texture, TileSizeX, TileSizeY, new Vector2(chunks[row].Count * TileSizeX, (chunks.Count - 1) * TileSizeY)));
                        }
                    }
                    //foreach (Chunk chunk in chunks[row])
                    //{
                    if (chunks[row][column].Bitmap == null)
                    {
                        Thread thread = new Thread(() =>
                        {
                            chunks[row][column].Calculate(Scale, Vector.Z, Vector.W, Dimension);
                        });

                        threads.Add(thread);
                        thread.Start();
                        while (thread.ThreadState != ThreadState.Running)
                            Thread.Sleep(2);
                        thread.IsBackground = true;
                    }
                    //}
                }
            }

            //for (int i = 0; i < chunksX * chunksY; i++)
            //{
            //    chunks.Add(new Chunk(Texture, TileSizeX - 1, TileSizeY - 1));
            //}

            //int temp = chunks.Count;


            //for (int i = 0; i < temp; i++)
            //{
            //    // / (chunksX * TileSizeX)
            //    int offsetY = (int)Math.Floor((float)i / chunksX) * TileSizeY;
            //    //int offsetX = (int)(Math.Ceiling((float)i / ResolutionX) * TileSizeX);
            //    int offsetX = i * TileSizeX % ResolutionX;
            //    int offsetYAddend = (int)Math.Floor((float)i / chunksX);

            //    int iTemp = i;

            //    Thread thread = new Thread(() => { bitmaps[iTemp] = chunks[iTemp].Calculate(Scale, Vector.Z, Vector.W); });
            //    threads.Add(thread);
            //    thread.Start();
            //    while (thread.ThreadState != ThreadState.Running)
            //        Thread.Sleep(2);
            //    thread.IsBackground = true;
            //}

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            cachedBitmap = MergeBitmaps(chunks);
            return cachedBitmap;
        }

        private Bitmap MergeBitmaps(List<List<Chunk>> chunks)
        {
            Bitmap returnBitmap = new Bitmap(ExtraMath.CeilingStep(bounds.Width + bounds.X, TileSizeX), ExtraMath.CeilingStep(bounds.Height + bounds.Y, TileSizeY));
            using (Graphics gfx = Graphics.FromImage(returnBitmap))
            {
                //TODO: make this use the offset
                foreach (List<Chunk> row in chunks)
                {
                    foreach (Chunk chunk in row)
                    {
                        if (bounds.IntersectsWith(chunk.Boundary))
                        {
                            gfx.DrawImageUnscaled(chunk.Bitmap, new Point((int)chunk.Position.X, (int)chunk.Position.Y));
                        }
                    }
                }
            }

            return returnBitmap;
        }
    }
}

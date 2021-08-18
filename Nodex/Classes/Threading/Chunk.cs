using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Nodex.Classes.TextureGenerators.Dimensions;

namespace Nodex.Classes.Threading
{
    public class Chunk
    {
        public ITexture Texture { get; }
        public int SizeX { get; }
        public int SizeY { get; }
        public Vector2 Position { get; }
        public Bitmap Bitmap { get; private set; }
        public Rectangle Boundary { get; }

        public Chunk(ITexture texture, int size, Vector2 position)
        {
            Texture = texture;
            SizeX = size;
            SizeY = size;
            Position = position;
            Boundary = new Rectangle((int)position.X, (int)position.Y, size, size);
        }
        public Chunk(ITexture texture, int xSize, int ySize, Vector2 position)
        {
            Texture = texture;
            SizeX = xSize;
            SizeY = ySize;
            Position = position;
            Boundary = new Rectangle((int)position.X, (int)position.Y, xSize, ySize);
        }

        public void Calculate(double scale)
        {
            Bitmap bitmap = new Bitmap(SizeX, SizeY);
            for (int x = 0; x < SizeX; x++)
            {
                double evaluateX = (x + Position.X) / scale;
                for (int y = 0; y < SizeY; y++)
                {
                    int pixelBrightness = (int)((Texture.Evaluate(evaluateX, (y + Position.Y) / scale) + 1) * 127.5);
                    bitmap.SetPixel(x, y, Color.FromArgb(pixelBrightness, pixelBrightness, pixelBrightness));
                }
            }

            Bitmap = bitmap;
        }
        public void Calculate(double scale, double offsetZ)
        {
            Bitmap bitmap = new Bitmap(SizeX, SizeY);
            double evaluateZ = offsetZ / scale;
            for (int x = 0; x < SizeX; x++)
            {
                double evaluateX = (x + Position.X) / scale;
                for (int y = 0; y < SizeY; y++)
                {
                    int pixelBrightness = (int)((Texture.Evaluate(evaluateX, (y + Position.Y) / scale, evaluateZ) + 1) * 127.5);
                    bitmap.SetPixel(x, y, Color.FromArgb(pixelBrightness, pixelBrightness, pixelBrightness));
                }
            }

            Bitmap = bitmap;
        }
        public void Calculate(double scale, double offsetZ = 0, double offsetW = 0, Dimension dimension = Dimension.TwoD)
        {
            Bitmap bitmap = new Bitmap(SizeX, SizeY);
            double evaluateZ = offsetZ / scale;
            double evaluateW = offsetW / scale;
            //for (int x = 0; x < SizeX; x++)
            //{
            //    double evaluateX = (x + Position.X) / scale;
            //    for (int y = 0; y < SizeY; y++)
            //    {
            //        int pixelBrightness = (int)((Texture.Evaluate(evaluateX, (y + Position.Y) / scale, evaluateZ, evaluateW) + 1) * 127.5);
            //        bitmap.SetPixel(x, y, Color.FromArgb(pixelBrightness, pixelBrightness, pixelBrightness));
            //    }
            //}
            //Fill Bitmap with black to pre-allocate memory
            using (Graphics gfx = Graphics.FromImage(bitmap))
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                gfx.FillRectangle(brush, 0, 0, SizeX, SizeY);
            }

            unsafe
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                //Random random = new Random();
                for (int y = 0; y < heightInPixels; y++)
                {
                    double evaluateY = (y + Position.Y) / scale;
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        byte pixelBrightness = (byte)((Texture.Evaluate((x + Position.X) / scale, evaluateY, evaluateZ, evaluateW) + 1) * 127.5);
                        //byte pixelBrightness = (byte)random.Next(0, 256);
                        // calculate new pixel value
                        currentLine[x] = pixelBrightness;
                        currentLine[x + 1] = pixelBrightness;
                        currentLine[x + 2] = pixelBrightness;
                    }
                }
                bitmap.UnlockBits(bitmapData);
            }

            Bitmap = bitmap;
        }

        public void ClearBitmap()
        {
            Bitmap = null;
        }
    }
}

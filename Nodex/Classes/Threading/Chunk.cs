using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        public void Calculate(double scale, double offsetZ, double offsetW)
        {
            Bitmap bitmap = new Bitmap(SizeX, SizeY);
            double evaluateZ = offsetZ / scale;
            double evaluateW = offsetW / scale;
            for (int x = 0; x < SizeX; x++)
            {
                double evaluateX = (x + Position.X) / scale;
                for (int y = 0; y < SizeY; y++)
                {
                    int pixelBrightness = (int)((Texture.Evaluate(evaluateX, (y + Position.Y) / scale, evaluateZ, evaluateW) + 1) * 127.5);
                    bitmap.SetPixel(x, y, Color.FromArgb(pixelBrightness, pixelBrightness, pixelBrightness));
                }
            }

            Bitmap = bitmap;
        }

        public void ClearBitmap()
        {
            Bitmap = null;
        }
    }
}

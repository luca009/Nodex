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

        public void Calculate(float scale, double offsetZ, double offsetW)
        {
            Bitmap bitmap = new Bitmap(SizeX, SizeY);
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    int pixelBrightness = (int)((Texture.Evaluate((x + Position.X) / scale, (y + Position.Y) / scale, offsetZ, offsetW) + 1) * 127.5);
                    bitmap.SetPixel(x, y, Color.FromArgb(pixelBrightness, pixelBrightness, pixelBrightness));
                    //Console.WriteLine($"{{{x},{y}}}: {pixelBrightness}");
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

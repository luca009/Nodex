using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes.Threading
{
    public class Chunk
    {
        public ITexture Texture { get; }
        public int SizeX { get; }
        public int SizeY { get; }

        public Chunk(ITexture texture, int size)
        {
            Texture = texture;
            SizeX = size;
            SizeY = size;
        }
        public Chunk(ITexture texture, int xSize, int ySize)
        {
            Texture = texture;
            SizeX = xSize;
            SizeY = ySize;
        }

        public Bitmap Calculate(float scale, double offsetX, double offsetY)
        {
            Bitmap bitmap = new Bitmap(SizeX, SizeY);
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    int pixelBrightness = (int)((Texture.Evaluate((x + offsetX) / scale, (y + offsetY) / scale) + 1) * 127.5);
                    bitmap.SetPixel(x, y, Color.FromArgb(pixelBrightness, pixelBrightness, pixelBrightness));
                }
            }

            return bitmap;
        }
    }
}

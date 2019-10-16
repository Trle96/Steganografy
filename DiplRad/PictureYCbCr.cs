using System.Drawing;

namespace DiplRad
{
    class PictureYCbCr
    {
        public ColorYCbCrBlock[,] blocks;
        public int width, height;
        public int displayWidth, displayHeight;

        public int GetDisplayWidth()
        {
            return displayWidth;
        }

        public int GetDisplayHeight()
        {
            return displayHeight;
        }

        public PictureYCbCr(Bitmap picture)
        {
            this.displayHeight = picture.Height;
            this.displayWidth = picture.Width;
            this.width = picture.Width / 8 + (picture.Width % 8 == 0 ? 0 : 1);
            this.height = picture.Height / 8 + (picture.Height % 8 == 0 ? 0 : 1);
            blocks = new ColorYCbCrBlock[width, height];

            for (int startingPointy = 0; startingPointy < height; startingPointy++)
            {
                for (int startingPointx = 0; startingPointx < width; startingPointx++)
                {
                    blocks[startingPointx, startingPointy] = new ColorYCbCrBlock();
                    for (int currentPixely = 0; currentPixely < 8; currentPixely++)
                    {
                        for (int currentPixelx = 0; currentPixelx < 8; currentPixelx++)
                        {
                            int x = startingPointx * 8 + currentPixelx;
                            int y = startingPointy * 8 + currentPixely;

                            if (x >= picture.Width || y >= picture.Height)
                            {
                                blocks[startingPointx, startingPointy].SetPixel(Color.Black, currentPixelx, currentPixely);
                            }
                            else
                            {
                                blocks[startingPointx, startingPointy].SetPixel(picture.GetPixel(x, y), currentPixelx, currentPixely);
                            }
                        }
                    }
                }
            }
        }
    }
}

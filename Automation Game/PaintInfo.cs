using Android.Graphics;

namespace Automation_Game
{
    class PaintInfo
    {
        public Canvas canvas;
        public Bitmap bitmap;
        public int posX;
        public int posY;

        public PaintInfo(Canvas canvas, Bitmap bitmap, int posX, int posY)
        {
            this.canvas = canvas;
            this.bitmap = bitmap;
            this.posX = posX;
            this.posY = posY;
        }
    }
}
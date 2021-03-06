using System.Threading.Tasks;

namespace Automation_Game
{
    public abstract class Moveable : IMoveable
    {
        protected float x;
        protected float y;
        protected MapDraw parent;
        protected MovementPacket packet;
        public abstract void MoveTo(int targetX, int targetY);

        public float GetX()
        {
            return x;
        }

        public void SetX(float x)
        {
            this.x = x;
        }

        public void SetY(float y)
        {
            this.y = y;
        }

        public float GetY()
        {
            return y;
        }

        public void Move(float dx, float dy)
        {
            x += dx;
            y += dy;
        }

        public MapDraw GetParent()
        {
            return parent;
        }
    }
}
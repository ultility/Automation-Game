namespace Automation_Game
{
    public class MovementPacket
    {
        public Moveable moving { get; }
        public int targetX;
        public int targetY;

        public MovementPacket(Moveable moving, int targetX, int targetY)
        {
            this.moving = moving;
            this.targetX = targetX;
            this.targetY = targetY;
        }
    }
}
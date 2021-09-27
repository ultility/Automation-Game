namespace Automation_Game
{
    public class Delivery
    {
        public Item item { get; }
        public int amount { get; set; }

        public Delivery(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }
}
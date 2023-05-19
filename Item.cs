using System;
namespace Final
{
	public class Item
	{
        public string Name { get; private set; }
        public double Heal { get; private set; }

        public Item(string name, double heal)
        {
            Name = name;
            Heal = heal;
        }

        public Item Clone()
        {
            return (Item)this.MemberwiseClone();
        }
    }
}
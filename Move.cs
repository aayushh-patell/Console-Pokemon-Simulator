using System;
namespace Final
{
	public class Move
	{
        public string Name { get; private set; }
        public double Damage { get; private set; }
        public ElementType Element { get; private set; }

        public Move(string name, double damage, ElementType element)
        {
            Name = name;
            Damage = damage;
            Element = element;
        }
    }
}
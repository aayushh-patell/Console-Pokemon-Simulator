using System;
namespace Final
{
	public class Pokemon
	{
        public string Name { get; private set; }
        public double BaseAttack { get; private set; }
        public double BaseDefense { get; private set; }
        public double BaseHP { get; private set; }
        public double CurrentHP { get; private set; }

        public ElementType Element { get; private set; }

        public Trainer Trainer { get; private set; }

        public List<Move> Moves { get; private set; }

        public Pokemon(string name, double baseStrength, double baseDefense, double baseHP, ElementType element)
		{
            Name = name;
            BaseAttack = baseStrength;
            BaseDefense = baseDefense;
            BaseHP = baseHP;
            CurrentHP = BaseHP;

            Trainer = null;
            Element = element;

            Moves = LearnMoves();
        }

        public Pokemon Clone()
        {
            return (Pokemon)this.MemberwiseClone();
        }

        // Selects a random move from a pokemon's set
        public Move SelectRandomMove()
        {
            Random random = new Random();
            int index = random.Next(0, Moves.Count);
            return Moves[index];
        }

        public void UseItem(Item item)
        {
            // Don't use item if at max hp
            if (this.CurrentHP == this.BaseHP)
            {
                Console.WriteLine($"{this.Name} is already at max HP");
            }
            // Heal pokemon until max hp
            else if ((this.CurrentHP + item.Heal) > this.BaseHP)
            {
                this.CurrentHP = this.BaseHP;
                Console.WriteLine($"{this.Name} was healed using a {item.Name} for {item.Heal} HP");
                Trainer.ItemCollection.Remove(item);
            }
            // Heal pokemon
            else
            {
                this.CurrentHP += item.Heal;
                Console.WriteLine($"{this.Name} was healed using a {item.Name} for {item.Heal} HP");
                Trainer.ItemCollection.Remove(item);
            }
        }

        public void SetTrainer(Trainer trainer)
        {
            Trainer = trainer;
        }

        public void RemoveTrainer(Trainer trainer)
        {
            Trainer = null;
        }

        // Take extra damage for critical attacks
        public void TakeCriticalDamage(double damage, GymLeader gymLeader, Trainer player, int receiver)
        {
            double finalDamage = Math.Round((18 + (damage * (this.BaseAttack / 4) / 3.25) - this.BaseDefense), 2);
            Console.WriteLine($"It was a critical strike dealing {finalDamage} damage");
            CurrentHP -= finalDamage;
            PrintAttackResult(gymLeader, player, receiver);
        }

        public void TakeDamage(double damage, GymLeader gymLeader, Trainer player, int receiver)
        {
            double finalDamage = Math.Round(((18 + (damage * (this.BaseAttack / 4.75)) / 3.25) - this.BaseDefense));
            Console.WriteLine($"It dealt {finalDamage} damage");
            CurrentHP -= finalDamage;
            PrintAttackResult(gymLeader, player, receiver);
        }

        // Take reduced damage for weak attacks
        public void TakeReducedDamage(double damage, GymLeader gymLeader, Trainer player, int receiver)
        {
            double finalDamage = Math.Round(((18 + (damage * this.BaseAttack) / 5.5) - this.BaseDefense));
            Console.WriteLine($"It was an ineffective strike dealing {finalDamage} damage");
            CurrentHP -= finalDamage;
            PrintAttackResult(gymLeader, player, receiver);
        }

        // Prints attack outcome
        private void PrintAttackResult(GymLeader gymLeader, Trainer player, int receiver)
        {
            if (CurrentHP <= 0 && receiver == 1)
            {
                Console.WriteLine($"{gymLeader.Name}'s {this.Name} fainted\n");
                CurrentHP = 0;
            }
            else if (CurrentHP <= 0 && receiver == 0)
            {
                Console.WriteLine($"{player.Name}'s {this.Name} fainted\n");
                CurrentHP = 0;
            }
            else if (CurrentHP > 0 && receiver == 1)
            {
                Console.WriteLine($"{gymLeader.Name}'s {this.Name}'s HP reduced to {this.CurrentHP}\n");
            }
            else if (CurrentHP > 0 && receiver == 0)
            {
                Console.WriteLine($"{player.Name}'s {this.Name}'s HP reduced to {this.CurrentHP}\n");
            }
        }

        // Teaches pokemon all moves corresponding to it element
        public List<Move> LearnMoves()
        {
            List<Move> moves = new List<Move>();

            foreach (Move move in Game.AllMoves)
            {
                if (move.Element == this.Element)
                {
                    moves.Add(move);
                }
            }

            return moves;
        }


        public void SetHP(double health)
        {
            CurrentHP = health;
        }
    }
}
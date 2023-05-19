using System;
namespace Final
{
	public class Battle
	{
        public Trainer Trainer { get; private set; }
        public Pokemon TrainerPokemon { get; private set; }

        public GymLeader GymLeader { get; private set; }
        public Pokemon GymLeaderPokemon { get; private set; }

        public int BattleWon { get; private set; }

        public Battle(Trainer trainer, GymLeader gymLeader)
		{
            Trainer = trainer;
            TrainerPokemon = Trainer.ActivePokemon;
            GymLeader = gymLeader;
            GymLeaderPokemon = GymLeader.PokemonCollection[0];

            // Revives all of the gym elader's pokemon at the beginning of each battle
            ResetGymLeaderPokemonHealth();

            // I used int rather than a bool due to an error i was facing
            BattleWon = 0;
        }

        public void TrainerTurn()
        {
            Move attack = Trainer.ActivePokemon.SelectRandomMove();
            // If the opponent's Pokémon's element is contained in the HashSet (value) of strengths of the attacking Pokémon, deal extra damage
            if (Game.Strengths[attack.Element].Contains(GymLeaderPokemon.Element))
            {
                Console.WriteLine($"{Trainer.Name}'s {TrainerPokemon.Name} used {attack.Name}");
                GymLeaderPokemon.TakeCriticalDamage(attack.Damage * TrainerPokemon.BaseAttack, GymLeader, Trainer, 1);
            }
            // If the opponent's Pokémon's element is contained in the HashSet (value) of weaknesses of the attacking Pokémon, deal less damage
            else if (Game.Weaknesses[attack.Element].Contains(GymLeaderPokemon.Element))
            {
                Console.WriteLine($"{Trainer.Name}'s {TrainerPokemon.Name} used {attack.Name}");
                GymLeaderPokemon.TakeReducedDamage(attack.Damage * TrainerPokemon.BaseAttack, GymLeader, Trainer, 1);
            }
            // Attack normally
            else
            {
                Console.WriteLine($"{Trainer.Name}'s {TrainerPokemon.Name} used {attack.Name}");
                GymLeaderPokemon.TakeDamage(attack.Damage * TrainerPokemon.BaseAttack, GymLeader, Trainer, 1);
            }

            // If the gym leader's pokemon is knocked out, check to see if any other pokemon from their collection can take its place
            if (GymLeaderPokemon.CurrentHP <= 0)
            {
                int willContinue = GymLeader.SetActivePokemonInBattle(this);

                if (willContinue == 1)
                {
                    // Gym leader no longer has pokemon to fight
                    BattleWon = 1;
                    Trainer.BattleHistory.Add(this);
                }
            }
        }

        public void OpponentTurn()
        {
            Move attack = GymLeaderPokemon.SelectRandomMove();

            // If the opponent's Pokémon's element is contained in the HashSet (value) of strengths of the attacking Pokémon, deal extra damage
            if (Game.Strengths[attack.Element].Contains(TrainerPokemon.Element))
            {
                Console.WriteLine($"{GymLeader.Name}'s {GymLeaderPokemon.Name} used {attack.Name}");
                TrainerPokemon.TakeCriticalDamage(attack.Damage, GymLeader, Trainer, 0);
            }
            // If the opponent's Pokémon's element is contained in the HashSet (value) of weaknesses of the attacking Pokémon, deal extra damage
            else if (Game.Weaknesses[attack.Element].Contains(TrainerPokemon.Element))
            {
                Console.WriteLine($"{GymLeader.Name}'s {GymLeaderPokemon.Name} used {attack.Name}");
                TrainerPokemon.TakeReducedDamage(attack.Damage, GymLeader, Trainer, 0);
            }
            // Attack normally
            else
            {
                Console.WriteLine($"{GymLeader.Name}'s {GymLeaderPokemon.Name} used {attack.Name}");
                TrainerPokemon.TakeDamage(attack.Damage, GymLeader, Trainer, 0);
            }

            // If the player's pokemon is knocked out, check to see if any other pokemon from their collection can take its place
            if (TrainerPokemon.CurrentHP <= 0)
            {
                int willContinue = Trainer.SetActivePokemonInBattle(this);

                if (willContinue == 1)
                {
                    // Player no longer has pokemon to fight
                    BattleWon = 2;
                    Trainer.BattleHistory.Add(this);
                } else
                {
                    Console.WriteLine($"{Trainer.Name} sent out {TrainerPokemon.Name}");
                }
            }
        }

        // Course of action for if the player wins the gym battle
        public void PlayerWon(Game game)
        {
            Console.WriteLine(GymLeader.LoseScript);

            if (game.GymLeaderIndex == Game.AllGymLeaders.Count)
            {
                // If the player just beat the last gym leader, the game is over
                game.ToggleGameWon();
            } else
            {
                // If the player still has more gym leaders to face, increment the index
                game.IncrementGymLeaderIndex();
            }
        }

        // Course of action for if the player loses the gym battle
        public void PlayerLost(Game game)
        {
            Console.WriteLine(GymLeader.WinScript);
        }

        // Method to rest health of each of the gym leader's pokemon at the beginning of a battle
        public void ResetGymLeaderPokemonHealth()
        {
            GymLeaderPokemon.SetHP(GymLeaderPokemon.BaseHP);

            foreach (Pokemon pokemon in GymLeader.PokemonCollection)
            {
                pokemon.SetHP(pokemon.BaseHP);
            }
        }

        // Sets gym leader's active combat pokemon
        public void SetGymLeaderPokemon(Pokemon pokemon)
        {
            GymLeaderPokemon = pokemon;
        }

        // Sets trainer's active combat pokemon
        public void SetTrainerPokemon(Pokemon pokemon)
        {
            TrainerPokemon = pokemon;
        }
    }
}
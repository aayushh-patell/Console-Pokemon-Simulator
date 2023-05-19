using System;
using System.Text;

namespace Final
{
	public class Trainer
	{
		public string Name { get; set; }

		public Pokemon ActivePokemon { get; private set; }
		public List<Pokemon> PokemonCollection { get; private set; }
		public List<Item> ItemCollection { get; private set; }
		public List<Battle> BattleHistory { get; private set; }

		public int GymBadges { get; private set; }

		public int Wins { get; private set; }
		public int Losses { get; private set; }

		public Trainer(string name)
		{
			Name = name;

			PokemonCollection = new List<Pokemon>();
			ItemCollection = new List<Item>();
			BattleHistory = new List<Battle>();

			Wins = 0;
			Losses = 0;
		}

		// Random outcome to catch pokemon
		public void CatchPokemon(Pokemon pokemon)
		{
			Pokemon newPokemon = pokemon.Clone();

			if (PokemonCollection.Count < 5)
			{
                Console.WriteLine("Pokéball was thrown ...");

                Random random = new Random();
                int outcome = random.Next(0, 2);

				if (outcome == 0)
				{
					Console.WriteLine($"... but the wild {pokemon.Name} escaped");
				}
				else
				{
                    PokemonCollection.Add(pokemon);
					Console.WriteLine($"... {pokemon.Name} was caught");
					pokemon.SetTrainer(this);
                }
            }
			else
			{
				Console.WriteLine("You do not have enough room in your collection for another Pokémon.");
			}
		}

		// Prints names and values of all pokemon a user owns
		public void PrintAllPokemon()
		{
			Console.WriteLine("Pokémon Collection:");

            Console.WriteLine($"{1} \t Name: {ActivePokemon.Name.PadRight(8)} \t Base Attack: {ActivePokemon.BaseAttack.ToString().PadRight(8)} \t Base Defense: {ActivePokemon.BaseDefense.ToString().PadRight(8)} \t Base HP: {ActivePokemon.BaseHP.ToString().PadRight(8)} \t Current HP: {ActivePokemon.CurrentHP.ToString().PadRight(8)} \t Element: {ActivePokemon.Element}");

            for (int i = 2; i < PokemonCollection.Count + 2; i++)
			{
				int j = i - 2;
				Console.WriteLine($"{i} \t Name: {PokemonCollection[j].Name.PadRight(8)} \t Base Attack: {PokemonCollection[j].BaseAttack.ToString().PadRight(8)} \t Base Defense: {PokemonCollection[j].BaseDefense.ToString().PadRight(8)} \t Base HP: {PokemonCollection[j].BaseHP.ToString().PadRight(8)} \t Current HP: {PokemonCollection[j].CurrentHP.ToString().PadRight(8)} \t Element: {PokemonCollection[j].Element}");
            }

        }

		// Moves onto next pokemon if one faints in battle
        public int SetActivePokemonInBattle(Battle battle)
        {
            for (int i = 0; i < PokemonCollection.Count; i++)
            {
                if (PokemonCollection[i].CurrentHP > 0)
                {
                    battle.SetTrainerPokemon(PokemonCollection[i]);
                    Console.WriteLine($"{this.Name} sent out {PokemonCollection[i].Name}\n");
                    return 0;
                }
            }

            return 1;
        }

		// Set a new active pokemon after battle
		public void SetActivePokemonAfterBattle(Pokemon pokemon)
		{
			PokemonCollection.Add(ActivePokemon);
			ActivePokemon = pokemon;
			PokemonCollection.Remove(pokemon);
		}

		// Sets starter pokemon
		public void SetStarter(Pokemon pokemon)
		{
			ActivePokemon = pokemon.Clone();
			pokemon.SetTrainer(this);
			Console.WriteLine($"{pokemon.Name} has joined your team");
		}
    }
}
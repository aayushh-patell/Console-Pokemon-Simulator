using System;
namespace Final
{
	public class GymLeader
	{
		public string Name { get; private set; }

        public Pokemon ActivePokemon { get; private set; }
        public List<Pokemon> PokemonCollection { get; private set; }

        public string OpenScript { get; private set; }
        public string WinScript { get; private set; }
        public string LoseScript { get; private set; }

        public GymLeader(string name, string openScript, string winScript, string loseScript, List<Pokemon> collection)
		{
			Name = name;
			OpenScript = openScript;
			WinScript = winScript;
			LoseScript = loseScript;
			PokemonCollection = collection;
			ActivePokemon = PokemonCollection[0];
		}

        public GymLeader Clone()
        {
            return (GymLeader)this.MemberwiseClone();
        }

        // Moves onto next pokemon if one faints in battle
        public int SetActivePokemonInBattle(Battle battle)
		{
			for (int i = 0; i < PokemonCollection.Count; i++)
			{
				if (PokemonCollection[i].CurrentHP > 0)
				{
					battle.SetGymLeaderPokemon(PokemonCollection[i]);
					Console.WriteLine($"{this.Name} sent out {PokemonCollection[i].Name}\n");
					return 0;
				}
			}

			return 1;
		}
	}
}


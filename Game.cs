using System;
namespace Final
{
	public class Game
	{
        public Trainer Player { get; private set; }

        // Index of encountered gym leaders
        public int GymLeaderIndex { get; private set; }

        public bool GameWon { get; private set; }

        public static List<Move> AllMoves { get; private set; } = new();
        public static List<Pokemon> AllWildPokemon { get; private set; } = new();
        public static List<Item> AllWildItems { get; private set; } = new();
        public static List<GymLeader> AllGymLeaders { get; private set; } = new();

        public static Dictionary<ElementType, HashSet<ElementType>> Weaknesses { get; private set; } = new();
        public static Dictionary<ElementType, HashSet<ElementType>> Strengths { get; private set; } = new();

        public Game(Trainer player)
        {
            Player = player;

            GymLeaderIndex = 0;
            GameWon = false;

            AllMoves = SetAllMoves();
            AllWildPokemon = SetAllWildPokemon();
            AllWildItems = SetAllWildItems();
            AllGymLeaders = SetAllGymLeaders(Player);

            Weaknesses = SetWeaknesses();
            Strengths = SetStrengths();
        }

        public void Play()
        {
            // Prompts user to choose an initial starter pokemon
            SelectStarter(Player);

            // Continuously shows menu until the user wins the game
            while (!GameWon)
            {
                ShowMenu();
            }
        }

        // Prompts user to select their very first pokemon
        public void SelectStarter(Trainer player)
        {
            bool validSelection = false;

            do
            {
                Console.WriteLine("Welcome to the wonderful world of Pokémon. Choose your starter Pokémon by selecting a number.");
                Console.WriteLine("1. Charmander");
                Console.WriteLine("2. Squirtle");
                Console.WriteLine("3. Bulbasaur");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Pokemon charmander = new Pokemon("Charmander", 7.2, 4.3, 39, ElementType.fire);
                        player.SetStarter(charmander);
                        validSelection = true;
                        break;
                    case "2":
                        Pokemon squirtle = new Pokemon("Squirtle", 6.8, 6.5, 44, ElementType.water);
                        player.SetStarter(squirtle);
                        validSelection = true;
                        break;
                    case "3":
                        Pokemon bulbasaur = new Pokemon("Bulbasaur", 6.9, 4.9, 45, ElementType.grass);
                        player.SetStarter(bulbasaur);
                        validSelection = true;
                        break;
                    default:
                        Console.WriteLine("Invalid menu selection");
                        break;
                }
            } while (!validSelection);
        }

        // Displays menu options
        public void ShowMenu()
        {
            bool isValidInput = false;

            do
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Show Player Stats");
                Console.WriteLine("2. Show Pokémon Inventory");
                Console.WriteLine("3. Heal a Pokémon");
                Console.WriteLine("4. Continue Journey");
                Console.WriteLine("5. Battle Gym Leader");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowPlayerStatistics();
                        isValidInput = true;
                        break;
                    case "2":
                        ShowPokemonInventory();
                        isValidInput = true;
                        break;
                    case "3":
                        // Checks to see if the player has any items to use
                        if (Player.ItemCollection.Count == 0)
                        {
                            Console.WriteLine("You do not have any items to use");
                        } else
                        {
                            ShowPokemonInventory();
                            HealAPokemon();
                        }
                        break;
                    case "4":
                        ContinueJourney();
                        isValidInput = true;
                        break;
                    case "5":
                        // Check to see if any pokemon are fit for battle (hp > 0)
                        if (IsBattlePossible() == true)
                        {
                            BattleGymLeader();
                        } else
                        {
                            Console.WriteLine("None of your Pokémon are fit for battle");
                        }
                        isValidInput = true;
                        break;
                    default:
                        Console.WriteLine("Invalid menu selection");
                        break;
                }
            } while (!isValidInput);
        }

        // Shows player statistics
        public void ShowPlayerStatistics()
        {
            Console.WriteLine($"Name: {Player.Name}");
            Console.WriteLine($"Active Pokémon: {Player.ActivePokemon.Name}");
            Console.WriteLine($"# of Pokémon: {Player.PokemonCollection.Count}");
        }

        // Calls method to show pokemon inventory and stats
        public void ShowPokemonInventory()
        {
            Player.PrintAllPokemon();
        }

        // User randomly encounters either a wild pokemon or a stray item to pick up
        public void ContinueJourney()
        {
            Console.WriteLine($"You have continued your journey ...");

            Random random = new Random();
            int outcome = random.Next(0, 2);

            switch (outcome)
            {
                case 0:
                    // Selects random wild pokemon to encounter
                    Pokemon wildPokemon = SelectWildPokemon();
                    Console.WriteLine($" ... You have encountered a wild {wildPokemon.Name}");

                    string pokInput = "";
                    int pokMenuOption = 0;
                    bool pokValid = false;

                    // Prompts user to either catch the pokemon or leave
                    while (!pokValid)
                    {
                        Console.WriteLine($"Would you like to catch the wild {wildPokemon.Name}? Select a menu option");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");

                        pokInput = Console.ReadLine();
                        int.TryParse(pokInput, out pokMenuOption);

                        switch (pokMenuOption)
                        {
                            case 1:
                                // Randomly generates outcome of encounter
                                Player.CatchPokemon(wildPokemon);
                                pokValid = true;
                                break;
                            case 2:
                                Console.WriteLine($"The wild {wildPokemon.Name} fled");
                                pokValid = true;
                                break;
                            default:
                                Console.WriteLine("Invalid menu selection");
                                break;
                        }
                    }
                    break;
                case 1:
                    // Randomly selects stray item
                    Item wildItem = SelectWildItem();
                    Player.ItemCollection.Add(wildItem);
                    Console.WriteLine($"... You have found a {wildItem.Name}");

                    string itemInput = "";
                    int itemMenuOption = 0;
                    bool itemValid = false;

                    // Prompts user to either pick up the item or leave
                    while (!itemValid)
                    {
                        Console.WriteLine($"Would you like to pick up the {wildItem.Name}? Select a menu option");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");

                        itemInput = Console.ReadLine();
                        int.TryParse(itemInput, out itemMenuOption);

                        switch (itemMenuOption)
                        {
                            case 1:
                                Player.ItemCollection.Add(wildItem);
                                Console.WriteLine($"{Player.Name} picked up a {wildItem.Name}");
                                itemValid = true;
                                break;
                            case 2:
                                Console.WriteLine($"A wild {SelectWildPokemon().Name} stole the {wildItem.Name} and fled");
                                itemValid = true;
                                break;
                            default:
                                Console.WriteLine("Invalid menu selection");
                                break;
                        }
                    }
                    break;
            }
        }

        // Heal a sleected pokemon with the first item in a list
        public void HealAPokemon()
        {
            bool valid = false;
            Item item = Player.ItemCollection[0];

            do
            {
                Console.WriteLine("Select the number corresponding with the Pokémon you wish to heal");

                string? stringInput = Console.ReadLine();

                int.TryParse(stringInput, out int menuSelection);

                if (menuSelection > (Player.PokemonCollection.Count + 1))
                {
                    continue;
                }

                switch (menuSelection)
                {
                    case 1:
                        Player.ActivePokemon.UseItem(item);
                        valid = true;
                        break;
                    case 2:
                        Player.PokemonCollection[0].UseItem(item);
                        valid = true;
                        break;
                    case 3:
                        Player.PokemonCollection[1].UseItem(item);
                        valid = true;
                        break;
                    case 4:
                        Player.PokemonCollection[2].UseItem(item);
                        valid = true;
                        break;
                    case 5:
                        Player.PokemonCollection[3].UseItem(item);
                        valid = true;
                        break;
                    case 6:
                        Player.PokemonCollection[4].UseItem(item);
                        valid = true;
                        break;
                }

            } while (!valid);
        }

        // Simulates gym battle
        public void BattleGymLeader()
        {
            // Plays gym leader's opening speech
            Console.WriteLine($"{Game.AllGymLeaders[GymLeaderIndex].OpenScript}\n");

            // Initialises new game
            Battle battle = new Battle(Player, Game.AllGymLeaders[GymLeaderIndex]);

            Console.WriteLine($"{Player.Name} sent out {battle.TrainerPokemon.Name}");
            Console.WriteLine($"{battle.GymLeader.Name} sent out {battle.GymLeaderPokemon.Name}\n");

            bool battleEnded = false;
            int turn = 1;

            // Alternates player turns until one party no longer has any pokemon left
            while (!battleEnded)
            {
                turn++;

                if (turn%2 == 0)
                {
                    battle.TrainerTurn();
                    if (battle.BattleWon == 1)
                    {
                        battle.PlayerWon(this);
                        battleEnded = true;
                        break;
                    }
                } else
                {
                    battle.OpponentTurn();
                    if (battle.BattleWon == 2)
                    {
                        battle.PlayerLost(this);
                        battleEnded = true;
                        break;
                    }
                }
            }
        }

        // Selects gym leader to battle based on storyline order
        public GymLeader SelectGymLeader()
        {
            return Game.AllGymLeaders[GymLeaderIndex].Clone();
        }

        // Randomly selects a wild pokemon to encounter
        private Pokemon SelectWildPokemon()
        {
            Random random = new Random();
            int index = random.Next(0, Game.AllWildPokemon.Count);
            return Game.AllWildPokemon[index].Clone();
        }

        // Randomly elects a stray item to find
        private Item SelectWildItem()
        {
            Random random = new Random();
            int index = random.Next(0, 3);
            return Game.AllWildItems[index].Clone();
        }

        // Increases gym leader index (responsible for order of encountered gym leaders)
        public void IncrementGymLeaderIndex()
        {
            GymLeaderIndex++;
        }

        // Checks to see if any pokemon have more than 0 hp to battle
        public bool IsBattlePossible()
        {
            // First checks active pokemon
            if (Player.ActivePokemon.CurrentHP > 0)
            {
                return true;
            }

            // If the active pokemon has no hp, check the rest of the pokemon collection
            foreach (Pokemon pokemon in Player.PokemonCollection)
            {
                if (pokemon.CurrentHP > 0)
                {
                    Player.SetActivePokemonAfterBattle(pokemon);
                    return true;
                }
            }

            return false;
        }

        // Ends game once the player beats the final gym leader
        public void ToggleGameWon()
        {
            GameWon = true;
        }

        // Database of possible wild pokemon
        private static List<Pokemon> SetAllWildPokemon()
        {
            Pokemon bulbasaur = new Pokemon("Bulbasaur", 6.9, 4.9, 45, ElementType.grass);
            Pokemon charmander = new Pokemon("Charmander", 7.2, 4.3, 39, ElementType.fire);
            Pokemon squirtle = new Pokemon("Squirtle", 6.8, 6.5, 44, ElementType.water);
            Pokemon caterpie = new Pokemon("Caterpie", 5.0, 5.5, 45, ElementType.bug);
            Pokemon weedle = new Pokemon("Weedle", 5.5, 5.0, 40, ElementType.bug);
            Pokemon pidgey = new Pokemon("Pidgey", 6.5, 6.0, 40, ElementType.flying);
            Pokemon rattata = new Pokemon("Rattata", 7.6, 5.5, 30, ElementType.normal);
            Pokemon spearow = new Pokemon("Spearow", 8.0, 5.0, 40, ElementType.flying);
            Pokemon ekans = new Pokemon("Ekans", 8.0, 6.4, 35, ElementType.poison);
            Pokemon sandshrew = new Pokemon("Sandshrew", 9.5, 10.5, 50, ElementType.ground);
            Pokemon nidoranF = new Pokemon("Nidoran", 6.7, 7.2, 55, ElementType.poison);
            Pokemon nidoranM = new Pokemon("Nidoran", 7.7, 6.0, 46, ElementType.poison);
            Pokemon vulpix = new Pokemon("Vulpix", 6.1, 6.0, 38, ElementType.fire);

            Pokemon geodude = new Pokemon("Geodude", 10.0, 12.0, 40, ElementType.ground);
            Pokemon onix = new Pokemon("Onix", 6.5, 18.0, 35, ElementType.ground);

            Pokemon staryu = new Pokemon("Staryu", 6.5, 7.5, 30, ElementType.water);
            Pokemon starmie = new Pokemon("Starmie", 9.5, 10.5, 60, ElementType.water);

            Pokemon voltorb = new Pokemon("Voltorb", 5.0, 7.0, 40, ElementType.electric);
            Pokemon pikachu = new Pokemon("Pikachu", 7.5, 6.0, 35, ElementType.electric);
            Pokemon raichu = new Pokemon("Raichu", 11.0, 7.5, 60, ElementType.electric);

            Pokemon rhyhorn = new Pokemon("Rhyhorn", 10.5, 11.5, 80, ElementType.ground);
            Pokemon dugtrio = new Pokemon("Dugtrio", 12.0, 7.0, 35, ElementType.ground);
            Pokemon nidoqueen = new Pokemon("Nidoqueen", 11.2, 10.7, 90, ElementType.poison);
            Pokemon nidoking = new Pokemon("Nidoking", 12.2, 9.7, 81, ElementType.poison);

            List<Pokemon> allWildPokemon = new List<Pokemon>() { bulbasaur, charmander, squirtle, caterpie, weedle, pidgey, rattata, spearow, ekans, sandshrew, nidoranF, nidoranM, vulpix, geodude, onix, staryu, starmie, voltorb, pikachu, raichu, rhyhorn, dugtrio, nidoqueen, nidoking };
            return allWildPokemon;
        }

        // Database of possible items
        private static List<Item> SetAllWildItems()
        {
            Item potion = new Item("Potion", 10);
            Item superPotion = new Item("Super Potion", 15);
            Item hyperPotion = new Item("Hyper Potion", 20);

            List<Item> allWildItems = new List<Item>() { potion, superPotion, hyperPotion };
            return allWildItems;
        }

        // Sets gym leader names, pokemon collections, and scripts for when the user battles them
        private static List<GymLeader> SetAllGymLeaders(Trainer player)
        {
            Pokemon geodude = new Pokemon("Geodude", 10.0, 12.0, 40, ElementType.ground);
            Pokemon onix = new Pokemon("Onix", 6.5, 18.0, 35, ElementType.ground);

            List<Pokemon> brockPokemon = new List<Pokemon>() { geodude, onix };
            GymLeader brock = new GymLeader("Brock", $"Welcome to the Pewter City Gym, {player.Name}. My name is Brock. It's time to battle.", $"You put up a good fight {player.Name}, but not good enough.", $"Outstanding job, {player.Name}. You have a lot of potential, young trainer. This is for you, the boulder badge.", brockPokemon);

            Pokemon staryu = new Pokemon("Staryu", 6.5, 7.5, 30, ElementType.water);
            Pokemon starmie = new Pokemon("Starmie", 9.5, 10.5, 60, ElementType.water);

            List<Pokemon> mistyPokemon = new List<Pokemon>() { staryu, starmie };
            GymLeader misty = new GymLeader("Misty", $"Welcome to the Cerulean City Gym, {player.Name}. My name is Misty. It's time to battle.", $"You put up a good fight {player.Name}, but not good enough.", $"Outstanding job, {player.Name}. You have a lot of potential, young trainer. This is for you, the cascade badge.", mistyPokemon);

            Pokemon voltorb = new Pokemon("Voltorb", 5.0, 7.0, 40, ElementType.electric);
            Pokemon pikachu = new Pokemon("Pikachu", 7.5, 6.0, 35, ElementType.electric);
            Pokemon raichu = new Pokemon("Raichu", 11.0, 7.5, 60, ElementType.electric);

            List<Pokemon> surgePokemon = new List<Pokemon>() { voltorb, pikachu, raichu };
            GymLeader surge = new GymLeader("Surge", $"Welcome to the Vermilion City Gym, {player.Name}. My name is Surge. It's time to battle.", $"You put up a good fight {player.Name}, but not good enough.", $"Outstanding job, {player.Name}. You have a lot of potential, young trainer. This is for you, the thunder badge.", surgePokemon);

            Pokemon rhyhorn = new Pokemon("Rhyhorn", 10.5, 11.5, 80, ElementType.ground);
            Pokemon dugtrio = new Pokemon("Dugtrio", 12.0, 7.0, 35, ElementType.ground);
            Pokemon nidoqueen = new Pokemon("Nidoqueen", 11.2, 10.7, 90, ElementType.poison);
            Pokemon nidoking = new Pokemon("Nidoking", 12.2, 9.7, 81, ElementType.poison);

            List<Pokemon> giovanniPokemon = new List<Pokemon>() { geodude, onix };
            GymLeader giovanni = new GymLeader("Giovanni", $"Welcome to the Viridian City Gym, {player.Name}. My name is Giovanni. It's time to battle.", $"You put up a good fight {player.Name}, but not good enough.", $"Outstanding job, {player.Name}. You have officially completed every gym challenge. This is for you, the earth badge. You are now a Pokémon master.", giovanniPokemon);

            List<GymLeader> allGymLeaders = new List<GymLeader>() { brock, misty, surge, giovanni };
            return allGymLeaders;
        }

        // Database of possible pokemon attacks
        private static List<Move> SetAllMoves()
        {
            Move razorLeaf = new Move("Razor Leaf", 2, ElementType.grass);
            Move solarBeam = new Move("Solar Beam", 4, ElementType.grass);

            Move ember = new Move("Ember", 2, ElementType.fire);
            Move flameThrower = new Move("Flame Thrower", 4, ElementType.fire);

            Move waterGun = new Move("Water Gun", 2, ElementType.water);
            Move hydroPump = new Move("Hydro Pump", 4, ElementType.water);

            Move struggleBug = new Move("Struggle Bug", 2, ElementType.bug);
            Move pinMissle = new Move("Pin Missle", 4, ElementType.bug);

            Move aerialAce = new Move("Aerial Ace", 2, ElementType.flying);
            Move braveBird = new Move("Brave Bird", 4, ElementType.flying);

            Move cut = new Move("Cut", 2, ElementType.normal);
            Move bodySlam = new Move("Body Slam", 4, ElementType.normal);

            Move poisonPoint = new Move("Poison Point", 2, ElementType.poison);
            Move acid = new Move("Acid", 4, ElementType.poison);

            Move discharge = new Move("Discharge", 2, ElementType.electric);
            Move thunderBolt = new Move("Thunder Bolt", 4, ElementType.electric);

            Move sandAttack = new Move("Sand Attack", 2, ElementType.ground);
            Move earthquake = new Move("Earthquake", 4, ElementType.ground);

            List<Move> allMoves = new List<Move>() { razorLeaf, solarBeam, ember, flameThrower, waterGun, hydroPump, struggleBug, pinMissle, aerialAce, braveBird, cut, bodySlam, poisonPoint, acid, discharge, thunderBolt, sandAttack, earthquake };
            return allMoves;
        }

        // Creates a dictionary of element types (key) paired with a hashset of elements (value) which they would be strong / resistant against
        private static Dictionary<ElementType, HashSet<ElementType>> SetStrengths()
        {
            HashSet<ElementType> bugStrength = new HashSet<ElementType>() { ElementType.grass, ElementType.dark, ElementType.psychic };
            HashSet<ElementType> darkStrength = new HashSet<ElementType>() { ElementType.ghost, ElementType.psychic };
            HashSet<ElementType> dragonStrength = new HashSet<ElementType>() { ElementType.dragon };
            HashSet<ElementType> electricStrength = new HashSet<ElementType>() { ElementType.flying, ElementType.water };
            HashSet<ElementType> fairyStrength = new HashSet<ElementType>() { ElementType.fighting, ElementType.dark, ElementType.dragon };
            HashSet<ElementType> fightingStrength = new HashSet<ElementType>() { ElementType.dark, ElementType.ice, ElementType.normal, ElementType.rock, ElementType.steel };
            HashSet<ElementType> fireStrength = new HashSet<ElementType>() { ElementType.bug, ElementType.grass, ElementType.ice, ElementType.steel };
            HashSet<ElementType> flyingStrength = new HashSet<ElementType>() { ElementType.bug, ElementType.fighting, ElementType.grass };
            HashSet<ElementType> ghostStrength = new HashSet<ElementType>() { ElementType.ghost, ElementType.psychic };
            HashSet<ElementType> grassStrength = new HashSet<ElementType>() { ElementType.ground, ElementType.rock, ElementType.water };
            HashSet<ElementType> groundStrength = new HashSet<ElementType>() { ElementType.electric, ElementType.fire, ElementType.poison, ElementType.rock, ElementType.steel };
            HashSet<ElementType> iceStrength = new HashSet<ElementType>() { ElementType.dragon, ElementType.flying, ElementType.grass, ElementType.ground };
            HashSet<ElementType> normalStrength = new HashSet<ElementType>();
            HashSet<ElementType> poisonStrength = new HashSet<ElementType>() { ElementType.fairy, ElementType.grass };
            HashSet<ElementType> psychicStrength = new HashSet<ElementType>() { ElementType.fighting, ElementType.poison };
            HashSet<ElementType> rockStrength = new HashSet<ElementType>() { ElementType.bug, ElementType.fire, ElementType.flying, ElementType.ice };
            HashSet<ElementType> steelStrength = new HashSet<ElementType>() { ElementType.fairy, ElementType.ice, ElementType.rock };
            HashSet<ElementType> waterStrength = new HashSet<ElementType>() { ElementType.fire, ElementType.ground, ElementType.rock };

            Dictionary<ElementType, HashSet<ElementType>> strengths = new Dictionary<ElementType, HashSet<ElementType>>();

            strengths.Add(ElementType.bug, bugStrength);
            strengths.Add(ElementType.dark, darkStrength);
            strengths.Add(ElementType.dragon, dragonStrength);
            strengths.Add(ElementType.electric, electricStrength);
            strengths.Add(ElementType.fairy, fairyStrength);
            strengths.Add(ElementType.fighting, fightingStrength);
            strengths.Add(ElementType.fire, fireStrength);
            strengths.Add(ElementType.flying, flyingStrength);
            strengths.Add(ElementType.ghost, ghostStrength);
            strengths.Add(ElementType.grass, grassStrength);
            strengths.Add(ElementType.ground, groundStrength);
            strengths.Add(ElementType.ice, iceStrength);
            strengths.Add(ElementType.normal, normalStrength);
            strengths.Add(ElementType.poison, poisonStrength);
            strengths.Add(ElementType.psychic, psychicStrength);
            strengths.Add(ElementType.rock, rockStrength);
            strengths.Add(ElementType.steel, steelStrength);
            strengths.Add(ElementType.water, waterStrength);

            return strengths;
        }

        // Creates a dictionary of element types (key) paired with a hashset of elements (value) which they would be weak against
        private static Dictionary<ElementType, HashSet<ElementType>> SetWeaknesses()
        {
            HashSet<ElementType> bugWeakness = new HashSet<ElementType>() { ElementType.fire, ElementType.flying, ElementType.rock };
            HashSet<ElementType> darkWeakness = new HashSet<ElementType>() { ElementType.bug, ElementType.fairy, ElementType.fighting };
            HashSet<ElementType> dragonWeakness = new HashSet<ElementType>() { ElementType.dragon, ElementType.fairy, ElementType.ice };
            HashSet<ElementType> electricWeakness = new HashSet<ElementType>() { ElementType.ground };
            HashSet<ElementType> fairyWeakness = new HashSet<ElementType>() { ElementType.poison, ElementType.steel };
            HashSet<ElementType> fightingWeakness = new HashSet<ElementType>() { ElementType.fairy, ElementType.flying, ElementType.psychic };
            HashSet<ElementType> fireWeakness = new HashSet<ElementType>() { ElementType.ground, ElementType.rock, ElementType.water };
            HashSet<ElementType> flyingWeakness = new HashSet<ElementType>() { ElementType.electric, ElementType.ice, ElementType.rock };
            HashSet<ElementType> ghostWeakness = new HashSet<ElementType>() { ElementType.dark, ElementType.ghost };
            HashSet<ElementType> grassWeakness = new HashSet<ElementType>() { ElementType.bug, ElementType.fire, ElementType.flying, ElementType.ice, ElementType.poison };
            HashSet<ElementType> groundWeakness = new HashSet<ElementType>() { ElementType.grass, ElementType.ice, ElementType.water };
            HashSet<ElementType> iceWeakness = new HashSet<ElementType>() { ElementType.fighting, ElementType.fire, ElementType.rock, ElementType.steel };
            HashSet<ElementType> normalWeakness = new HashSet<ElementType>() { ElementType.fighting };
            HashSet<ElementType> poisonWeakness = new HashSet<ElementType>() { ElementType.ground, ElementType.psychic };
            HashSet<ElementType> psychicWeakness = new HashSet<ElementType>() { ElementType.bug, ElementType.dark, ElementType.ghost };
            HashSet<ElementType> rockWeakness = new HashSet<ElementType>() { ElementType.fighting, ElementType.grass, ElementType.ground, ElementType.steel, ElementType.water };
            HashSet<ElementType> steelWeakness = new HashSet<ElementType>() { ElementType.fighting, ElementType.fire, ElementType.ground };
            HashSet<ElementType> waterWeakness = new HashSet<ElementType>() { ElementType.electric, ElementType.grass };

            Dictionary<ElementType, HashSet<ElementType>> weaknesses = new Dictionary<ElementType, HashSet<ElementType>>();

            weaknesses.Add(ElementType.bug, bugWeakness);
            weaknesses.Add(ElementType.dark, darkWeakness);
            weaknesses.Add(ElementType.dragon, dragonWeakness);
            weaknesses.Add(ElementType.electric, electricWeakness);
            weaknesses.Add(ElementType.fairy, fairyWeakness);
            weaknesses.Add(ElementType.fighting, fightingWeakness);
            weaknesses.Add(ElementType.fire, fireWeakness);
            weaknesses.Add(ElementType.flying, flyingWeakness);
            weaknesses.Add(ElementType.ghost, ghostWeakness);
            weaknesses.Add(ElementType.grass, grassWeakness);
            weaknesses.Add(ElementType.ground, groundWeakness);
            weaknesses.Add(ElementType.ice, iceWeakness);
            weaknesses.Add(ElementType.normal, normalWeakness);
            weaknesses.Add(ElementType.poison, poisonWeakness);
            weaknesses.Add(ElementType.psychic, psychicWeakness);
            weaknesses.Add(ElementType.rock, rockWeakness);
            weaknesses.Add(ElementType.steel, steelWeakness);
            weaknesses.Add(ElementType.water, waterWeakness);

            return weaknesses;
        }
    }
}
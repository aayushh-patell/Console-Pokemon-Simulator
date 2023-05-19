using System;
using Final;

namespace Final
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Trainer ash = new Trainer("Ash");
            Game game = new Game(ash);

            game.Play();
        }
    }
}
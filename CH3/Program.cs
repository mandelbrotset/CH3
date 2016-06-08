using System;



namespace CH3
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            Game game = new Game();

            game.run(60);
        }

    }
}

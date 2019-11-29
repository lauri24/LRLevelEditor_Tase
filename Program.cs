using System;
namespace test
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            
            using (var game = new monoGameCP.Game1())
                game.Run();
        }
    }
}

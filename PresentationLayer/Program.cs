using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            var package = new Package();
            package.PrintCards();

            var user = new User("player1");
            var user2 = new User("player2");

            user.PrintStack();

            Console.WriteLine("Buy Package");
            user.BuyPackage();
            user.BuyPackage();

            user2.BuyPackage();

            user.PrintStack();

            user.SelectDeck("1;3;5;7");
            user2.SelectDeck("0;1;2;3");

            var battleController = new BattleController(user, user2);

            battleController.StartBattle();

            user.PrintDeck();
            user2.PrintDeck();

            Console.ReadKey();
            */

            var server = new HttpServer();
            server.StartAsync();

            Console.WriteLine("Press ENTER to stop the server...");
            Console.ReadLine(); // Wartet darauf, dass der Benutzer ENTER drückt

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var package = new Package();
            package.PrintCards();

            var user = new User();

            user.PrintStack();

            Console.WriteLine("Buy Package");
            user.BuyPackage();
            user.BuyPackage();

            user.PrintStack();

            user.SelectDeck("1;3;5;7");

            user.PrintDeck();

            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.PresentationLayer
{
    internal class RequestHandler
    {
        public string HandleRequest(string request, string httpMethod)
        {
            Console.WriteLine($"Handle Request {request} {httpMethod}");

            // Unterscheidung nach HTTP-Methode
            if (httpMethod == "POST" && request == "/login")
            {
                Console.WriteLine("Do Login");
                return "Do Login";
            }

            if (httpMethod == "GET" && request == "/user")
            {
                var user = new User("player1");
                var result = "<html><body>";
                result += $"<p>username: {user.GetName()}, coins: {user.Coins}</p>";
                result += "</body></html>";
                return result;
            }

            return "No Enpoint  found";
        }
    }
}

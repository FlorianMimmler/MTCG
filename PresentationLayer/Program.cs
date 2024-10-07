using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new HttpServer();
            _ = server.StartAsync();

            Console.WriteLine("Press ENTER to stop the server...");

            Console.ReadLine();

            server.Stop();

            Thread.Sleep(500);
        }
    }
}

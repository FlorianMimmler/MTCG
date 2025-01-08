
namespace MTCG.PresentationLayer
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

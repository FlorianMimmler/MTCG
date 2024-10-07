using MTCG.PresentationLayer;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{

    public class HttpServer
    {
        private const int ServerPort = 8080;
        private readonly RequestHandler _requestHandler = new RequestHandler();

        public async Task StartAsync()
        {
            var listener = new TcpListener(IPAddress.Any, ServerPort);
            listener.Start();
            Console.WriteLine("Server started, listening on port 8080...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");

                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                using var stream = client.GetStream();

                
                var buffer = new byte[client.ReceiveBufferSize];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                var body = request.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None)[1];

                var requestLines = request.Split(new string[] {"\r\n"}, StringSplitOptions.None);
                var httpMethod = requestLines[0].Split(' ')[0];
                var requestUrl = requestLines[0].Split(' ')[1];

                
                var response = _requestHandler.HandleRequest(requestUrl, httpMethod, body);

                var responseBuffer = Encoding.UTF8.GetBytes($"HTTP/1.1 {(int) response.Item1} {response.Item1.GetDescription()}\r\nContent-Length: {response.Item2.Length}\r\n\r\n{response.Item2}");
                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
        }

    }
}

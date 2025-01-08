using MTCG.PresentationLayer;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class HttpServer
    {
        private const int ServerPort = 10001;
        private readonly RequestHandler _requestHandler = new RequestHandler();
        private bool _running = false;
        private TcpListener _listener;

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, ServerPort);
            _listener.Start();
            Console.WriteLine($"Server started, listening on port {ServerPort}...");

            _running = true;

            try
            {
                while (_running)
                {
                    var acceptTask = _listener.AcceptTcpClientAsync();
                    if (await Task.WhenAny(acceptTask, Task.Delay(100)) == acceptTask)
                    {
                        var client = acceptTask.Result;
                        Console.WriteLine("Client connected.");

                        _ = Task.Run(() => HandleClientAsync(client));
                    }

                }

            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Listener stopped.");
            }


        }

        public void Stop()
        {
            _running = false;
            _listener?.Stop();
            Console.WriteLine("Server is stopping");
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                using var stream = client.GetStream();


                var buffer = new byte[client.ReceiveBufferSize];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var requestLines = request.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                var authorizationToken = (from line in requestLines where line.StartsWith("Authorization:", StringComparison.OrdinalIgnoreCase) select line.Substring("Authorization:".Length).Trim()).FirstOrDefault();

                var body = request.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None)[1];

                var httpMethod = requestLines[0].Split(' ')[0];
                var requestUrl = requestLines[0].Split(' ')[1];


                var response = await _requestHandler.HandleRequest(requestUrl, httpMethod, body, authorizationToken);

                var responseBuffer = Encoding.UTF8.GetBytes(response.ToString());
                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
        }

    }
}

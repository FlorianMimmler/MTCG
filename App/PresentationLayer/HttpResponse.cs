using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.PresentationLayer
{
    public class HttpResponse
    {

        public HttpStatusCode StatusCode { get; set; }
        public string ResponseText { get; set; } = "";

        public override string ToString()
        {
            return
                $"HTTP/1.1 {(int)StatusCode} {StatusCode.GetDescription()}\r\n" +
                $"Content-Length: {ResponseText.Length}\r\n" +
                $"Content-Type: application/json\r\n\r\n" +
                $"{ResponseText}";
        }

    }
}

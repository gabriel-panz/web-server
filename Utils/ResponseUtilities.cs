using System.Net;
using System.Net.Sockets;
using System.Text;

namespace web_server.Utils;

public static class ResponseUtilities
{
    public static void Respond(this NetworkStream stream, HttpStatusCode statusCode, string? content, string headers = "")
    {
        if (String.IsNullOrEmpty(content))
            content = "";

        string response = $"HTTP/1.0 {(int)statusCode}" + Environment.NewLine;

        if (!String.IsNullOrWhiteSpace(headers) && !headers.EndsWith(Environment.NewLine))
            headers += Environment.NewLine;

        headers += $"Content-Length: {content.Length}" + Environment.NewLine
            + $"Content-Type: text/html" + Environment.NewLine;

        if (!content.StartsWith(Environment.NewLine))
            content = Environment.NewLine + content;

        byte[] bytes = Encoding.ASCII.GetBytes(response + headers + content);
        stream.Write(bytes);
    }

}
using System.Net;
using System.Net.Sockets;
using System.Text;
using web_server.Utils;

IPAddress ip = new(new byte[] { 127, 0, 0, 1 });
TcpListener listener = new(ip, 8000);

listener.Start();
System.Console.WriteLine("server is on");

while (true)
{
    var client = listener.AcceptTcpClient();

    var buffer = new byte[1024];
    var stream = client.GetStream();
    stream.Read(buffer);

    var request = Encoding.ASCII.GetString(buffer);
    var pageFilePath = "./pages/index.html";

    if (request.StartsWith("GET"))
        pageFilePath = "./pages" + request.Split(" ")[1];

    string result = "";

    if (request.Split(" ").Length <= 1) continue;

    // Redirects to index when path is: '/'
    if (request.Split(" ")[1] == "/")
    {
        var locationHeader = "Location: /index.html" + Environment.NewLine;
        stream.Respond(HttpStatusCode.Redirect, null, locationHeader);
    }

    // Populates index with all the accessible resources
    if (request.Split(" ")[1] == "/index.html")
    {
        result = File.ReadAllText(pageFilePath);
        var pagesList = Directory.EnumerateFiles("./pages/");
        string pageLinks = "";
        foreach (var page in pagesList)
        {
            string pageUrl = page.Remove(0, 8);
            pageLinks += $"<li><a href=\"/{pageUrl}\">{pageUrl}</a></li>{Environment.NewLine}{Environment.NewLine}";
        }
        result = result.Replace("%PAGES_LIST%", pageLinks);

        stream.Respond(HttpStatusCode.OK, content: result);
    }

    if (File.Exists(pageFilePath))
        stream.Respond(HttpStatusCode.OK, content: File.ReadAllText(pageFilePath));
    else
        stream.Respond(HttpStatusCode.NotFound, content: File.ReadAllText("./pages/notfound.html"));

    client.Dispose();
}

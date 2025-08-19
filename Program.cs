using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;

var appRootPath = Directory.GetCurrentDirectory()
    + $"{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}";

var configuration = new ConfigurationBuilder()
    .SetBasePath(appRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var hostName = configuration["ServerHost"];
var port = configuration["Server_port"];
IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(hostName);
IPAddress ipAddress;
if (ipHostInfo.AddressList.Length >= 2)
{
    ipAddress = ipHostInfo.AddressList[1];
}
else
{
    ipAddress = IPAddress.Parse("127.0.0.1");
}
var endPoint = new IPEndPoint(ipAddress, int.Parse(port));

using TcpClient client = new();
await client.ConnectAsync(endPoint);
await using NetworkStream stream = client.GetStream();

var buffer = new byte[1_024];
int received = await stream.ReadAsync(buffer);

var message = Encoding.UTF8.GetString(buffer, 0, received);
Console.WriteLine($"Message received: \"{message}\"");
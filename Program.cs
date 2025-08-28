using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

var appRootPath = Directory.GetCurrentDirectory()
    + $"{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}";

var configuration = new ConfigurationBuilder()
    .SetBasePath(appRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var hostName = configuration["ServerHost"];
var port = configuration["ServerPort"];
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

string inputDataFllePath = appRootPath + configuration["InputPath"];
try
{
    using (StreamReader reader = new StreamReader(inputDataFllePath))
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(line);
            stream.Write(byteArray);
            Console.WriteLine($"Message sended: \"{line}\"");
        }
    }
}
catch (IOException e)
{
    Console.WriteLine($"Error reading file: {e.Message}");
}

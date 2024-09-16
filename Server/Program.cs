using Server;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var port = 27001;
var ip = IPAddress.Parse("10.1.18.8");

var ep = new IPEndPoint(ip, port);

using var listener = new TcpListener(ep);

listener.Start();

while (true)
{
    var client = listener.AcceptTcpClient();

    var procList = Process.GetProcesses().Select(p => new ProcessDTO(p.Id, p.ProcessName)).ToList();
    var option = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(procList, option);

    byte[] byteArray = Encoding.UTF8.GetBytes(json);

    var stream = client.GetStream();
    stream.Write(byteArray, 0, byteArray.Length);



}



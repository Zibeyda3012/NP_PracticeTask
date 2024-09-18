using Server;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var port = 27001;
var port1 = 27000;
var ip = IPAddress.Parse("10.1.18.8");

var ep = new IPEndPoint(ip, port);
var ep1 = new IPEndPoint(ip, port1);

using var listener = new TcpListener(ep);
using var listener1 = new TcpListener(ep1);

listener.Start();

while (true)
{
    var client = listener.AcceptTcpClient();

    var procList = Process.GetProcesses().Select(p => new ProcessDTO(p.Id, p.ProcessName)).ToList();
    var option = new JsonSerializerOptions { WriteIndented = true };

    var stream = client.GetStream();

    for (int i = 0; i < procList.Count; i++)
    {

        var json = JsonSerializer.Serialize(procList[i], option);
        byte[] byteArray = Encoding.UTF8.GetBytes(json);
        stream.Write(byteArray, 0, byteArray.Length);

    }

    client = listener1.AcceptTcpClient();

    stream = client.GetStream();

    var reader = new StreamReader(stream);
    var result = reader.ReadToEnd();
    var command = JsonSerializer.Deserialize<Command>(result);

    if (command.Type == "Start")
    {
        var process = Process.GetProcessById(command.ProcessId);
        process.Start();
    }

    else if (command.Type == "Kill")
    {
        var process = Process.GetProcessById(command.ProcessId);
        process.Kill();
    }

}



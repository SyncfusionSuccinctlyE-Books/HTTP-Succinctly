using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class GetSocket
{
    public static void Main(string[] args)
    {
        var host = "www.wikipedia.org";
        var resource = "/";
        
        Console.WriteLine("Connecting to {0}", host);

        if(args.GetLength(0) >= 2)
        {
            host = args[0];
            resource = args[1];
        }
        
        var result = GetResource(host, resource);
        Console.WriteLine(result);
    }

    private static string GetResource(string host, string resource)
    {
        var hostEntry = Dns.GetHostEntry(host);
        var socket = CreateSocket(hostEntry);
        SendRequest(socket, host, resource);
        return GetResponse(socket);
    }

    private static Socket CreateSocket(IPHostEntry hostEntry)
    {
        const int httpPort = 80;
        foreach (var address in hostEntry.AddressList)
        {
            var endPoint = new IPEndPoint(address, httpPort);
            var socket = new Socket(
                           endPoint.AddressFamily, 
                           SocketType.Stream, 
                           ProtocolType.Tcp);
            socket.Connect(endPoint);
            if (socket.Connected)
            {
                return socket;
            }
        }
        return null;
    }

    private static void SendRequest(Socket socket, string host, 
                                    string resource)
    {                
        var requestMessage = String.Format(
            "GET {0} HTTP/1.1\r\n" +
            "Host: {1}\r\n" +
            "\r\n",
            resource, host
        );

        var requestBytes = Encoding.ASCII.GetBytes(requestMessage);
        socket.Send(requestBytes);
    }    

    private static string GetResponse(Socket socket)
    {
        int bytes = 0;
        byte[] buffer = new byte[256];
        var result = new StringBuilder();
        
        do
        {
            bytes = socket.Receive(buffer);
            result.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
        } while (bytes > 0);

        return result.ToString();
    }
} 

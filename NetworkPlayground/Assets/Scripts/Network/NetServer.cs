using Assets.Scripts.Network.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetServer : MonoBehaviour
{
    public static NetServer Instance;

    public Socket Server;
    public int Port;

    public int TotalRead;
    public int TotalSent;

    public List<Client> Clients;

	void Start ()
    {
        Instance = this;
        Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Clients = new List<Client>();

        Listen();
	}

    public void Listen()
    {
        Server.Bind(new IPEndPoint(IPAddress.Loopback, Port));
        Server.Listen(8);

        Debug.Log("Server is listening");

        Server.BeginAccept(BeginAcceptCallback, null);
    }
    public void Send(Client client, string data)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        client.Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, BeginSendCallback, client);
        Debug.Log("Sending data");
    }

    void BeginAcceptCallback(IAsyncResult ar)
    {
        Client client = new Client(Server.EndAccept(ar));
        Clients.Add(client);

        Debug.Log("Accepted socket");
        client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, BeginReceiveCallback, client);

        Server.BeginAccept(BeginAcceptCallback, null);
    }
    void BeginReceiveCallback(IAsyncResult ar)
    {
        Client client = (Client)ar.AsyncState;
        int read = client.Socket.EndReceive(ar);
        TotalRead += read;

        if(client.Socket.Available > 0)
        {
            client.Data.AddRange(client.Buffer);
            client.Buffer = new byte[client.Buffer.Length];

            client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, BeginReceiveCallback, client);
            Debug.Log("More data to read");
        }
        else
        {
            client.Buffer = new byte[client.Buffer.Length];
            string data = Encoding.UTF8.GetString(client.Data.ToArray());
            client.Data = new List<byte>();

            //Implement data handling with json

            client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, BeginReceiveCallback, client);
            Debug.Log("Data read");
        }
    }
    void BeginSendCallback(IAsyncResult ar)
    {
        Client client = (Client)ar.AsyncState;
        int sent = client.Socket.EndSend(ar);
        TotalSent += sent;
        Debug.Log("Data sent");
    }
}

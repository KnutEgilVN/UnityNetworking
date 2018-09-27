using Assets.Scripts.Network.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetClient : MonoBehaviour
{
    public static NetClient Instance;

    public Client Client;
    public string RemoteIP;
    public int RemotePort;

    public int TotalRead;
    public int TotalSent;

    private IPEndPoint RemoteEP;

    void Start()
    {
        Instance = this;
        RemoteEP = new IPEndPoint(IPAddress.Parse(RemoteIP), RemotePort);

        Client = new Client(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));

        Connect(RemoteEP);
    }

    public void Connect(IPEndPoint ep)
    {
        Debug.Log("Client is connecting");

        Client.Socket.BeginConnect(ep, BeginConnectCallback, null);
    }
    public void Send(string data)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        Client.Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, BeginSendCallback, null);
        Debug.Log("Sending data");
    }
    
    void BeginConnectCallback(IAsyncResult ar)
    {
        Client.Socket.EndConnect(ar);
        Debug.Log("Client connected");
        Send(string.Format("\"id\": 0, \"name\": \"{0}\"", "NetClient"));
    }
    void BeginReceiveCallback(IAsyncResult ar)
    {
        int read = Client.Socket.EndReceive(ar);
        TotalRead += read;

        if (Client.Socket.Available > 0)
        {
            Client.Data.AddRange(Client.Buffer);
            Client.Buffer = new byte[Client.Buffer.Length];

            Client.Socket.BeginReceive(Client.Buffer, 0, Client.Buffer.Length, SocketFlags.None, BeginReceiveCallback, null);
            Debug.Log("[Client] More data to read");
        }
        else
        {
            Client.Buffer = new byte[Client.Buffer.Length];
            string data = Encoding.UTF8.GetString(Client.Data.ToArray());
            Client.Data = new List<byte>();

            //Implement data handling with json

            Client.Socket.BeginReceive(Client.Buffer, 0, Client.Buffer.Length, SocketFlags.None, BeginReceiveCallback, null);
            Debug.Log("[Client] read");
        }
    }
    void BeginSendCallback(IAsyncResult ar)
    {
        int sent = Client.Socket.EndSend(ar);
        TotalSent += sent;
        Debug.Log("[Client] sent");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Assets.Scripts.Network.Models
{
    public class Client
    {
        //ADD SOCKET IDS
        public Socket Socket;
        public List<byte> Data;
        public byte[] Buffer;

        public Client(Socket socket)
        {
            Socket = socket;
            Data = new List<byte>();
            Buffer = new byte[1024];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTcp4;
using EasyTcp4.ClientUtils;
using Godot;

namespace FFA.Empty.Empty
{
    public class Client
    {
        private EasyTcpClient client;


        public Client(string connexionIP)
        {
            client = new EasyTcpClient();

            client.OnDataReceive += DataRecieved;
            client.OnDisconnect += Disconnected;
            client.OnConnect += Connected;
            client.OnError += InternalClientError;

            if (!client.Connect(connexionIP, 1404)) throw new ArgumentException("could not connect to server");

            

        }

        public void ShutDownClient(){ client.Dispose(); }
        //Event Methods
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        private void InternalClientError(object sender, Exception e)
        {
            GD.Print("[Client] internal client error, shutting down");
        }

        private void Disconnected(object sender, EasyTcpClient e)
        {
            GD.Print("[Client] disconnected from server");
        }

        private void Connected(object sender, EasyTcpClient e)
        {
            GD.Print("[Client] connected to server");
        }

        private void DataRecieved(object sender, Message e)
        {
            byte[] data = e.Data;
        }
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Event Methods
    }
}

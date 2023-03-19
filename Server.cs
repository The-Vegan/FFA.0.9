using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTcp4;
using Godot;

namespace FFA.Empty.Empty
{
    public class Server
    {
        private EasyTcpServer server;
        private EasyTcpClient[] clientList = new EasyTcpClient[16];
        public Server()
        {
            server = new EasyTcpServer();

            server.OnConnect += ClientConnected;
            server.OnDisconnect += ClientDisconnected;
            server.OnError += InternalServerError;
            server.OnDataReceive += DataRecieved;



        }


        public void ShutDownServer(){ server.Dispose(); }

        //Event Methods
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        private void ClientConnected(object sender, EasyTcpClient e)
        {
            for(byte i = 0; i < clientList.Length; i++)
            {
                if(clientList[i] == null)
                {
                    //Stores the client in first avaliable spot
                    clientList[i] = e;
                    return;
                }
            }
            GD.Print("[Server] Client denied, Server is full");
            server.FireOnDisconnect(e);
        }

        private void ClientDisconnected(object sender, EasyTcpClient e)
        {
            for (byte i = 0; i < clientList.Length; i++)
            {
                if (clientList[i] == e)
                {
                    //remove the client from the list
                    clientList[i] = null;
                    GD.Print("[Server] Disconnected client removed from list");
                    return;
                }
            }
            GD.Print("[Server] Alien client Disconnected");
        }

        private void InternalServerError(object sender, Exception e)
        {
            GD.Print("[Server] Internal server error, shutting down");
            this.ShutDownServer();
        }

        private void DataRecieved(object sender, Message e)
        {
            if (!clientList.Contains(e.Client)) 
            { 
                GD.Print("[Server] data recieved from alien client");
                server.FireOnDisconnect(e.Client);
            } 
            byte[] data = e.Data;
        }
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Event Methods
    }
}

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
        //Packet constants
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        /*CLIENT -> SERVER*/
        private const byte MOVE = 1;
        private const byte SET_CHARACTER = 2;
        private const byte CHOSE_TEAM = 3;
        /*SERVER -> CLIENT*/
        //Pre launch
        private const byte ABOUT_TO_LAUNCH = 255;
        private const byte ABORT_LAUNCH = 254;
        private const byte LAUNCH = 253;
        private const byte SET_CLIENT_OR_ENTITY_ID = 252;
        private const byte SEND_NAME_LIST = 251;
        private const byte SET_LEVEL_CONFIG = 250;
        //Post launch
        private const byte GAME_OVER = 249;
        private const byte GAME_SOON_OVER = 248;
        private const byte SET_MOVES = 247;
        private const byte SYNC = 246;
        private const byte ITEM_GIVEN_BY_SERVER = 245;
        private const byte BLUNDERED_BY_SERVER = 244;
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Packet constants

        private EasyTcpClient client;

        private List<ClientData> allClients = new List<ClientData> ();


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

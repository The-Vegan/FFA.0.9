using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EasyTcp4;
using EasyTcp4.ServerUtils;
using Godot;

namespace FFA.Empty.Empty
{
    public class Server
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
        private const byte SEND_CLIENT_LIST = 251;
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


        private EasyTcpServer server;
        private EasyTcpClient[] clientList = new EasyTcpClient[16];
        public Server()
        {
            server = new EasyTcpServer();

            server.OnConnect += ClientConnected;
            server.OnDisconnect += ClientDisconnected;
            server.OnError += InternalServerError;
            server.OnDataReceive += DataRecieved;

            server.Start(1404);
            GD.Print("[Server] ip : " + (Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString()));

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
                    GD.Print("[Server] Client added in list, at " + i);
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


            switch (data[0])
            {
                case MOVE:
                    break;
                case SET_CHARACTER:
                    break;
                case CHOSE_TEAM:
                    break;

            }


        }
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Event Methods

        //NetworkStruct Generator
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        public ClientData ClientDataFromBytes(byte[] serializedClientData, int offset)
        {
            ClientData c = new ClientData();

            c.clientID = serializedClientData[offset];
            c.characterID = serializedClientData[offset + 1];
            c.team = serializedClientData[offset + 2];

            c.name = Encoding.Unicode.GetString(serializedClientData, (offset + 4), serializedClientData[offset + 3]);


            return c;
        }

        public EntitySync ESyncFromBytes(byte[] serializedSyncData, int offset)
        {
            EntitySync es = new EntitySync();

            es.health = (short)((serializedSyncData[offset] << 8) + serializedSyncData[offset + 1]);
            es.itemBar = serializedSyncData[offset + 2];
            es.blunderBar = serializedSyncData[offset + 3];
            es.heldItem = serializedSyncData[offset + 4];

            es.coordinate = new Vector2(serializedSyncData[offset + 5], serializedSyncData[offset + 6]);

            es.stun = serializedSyncData[offset + 7];
            es.blundered = (serializedSyncData[offset + 8] != 0);
            es.cooldown = serializedSyncData[offset + 9];

            return es;
        }

        public DamageTileSync DamageTileSyncFromBytes(byte[] data, int offset)
        {
            DamageTileSync ret = new DamageTileSync();
            ret.sourceID = data[offset];
            ret.damage = (short)((data[offset + 1] << 8) + data[offset + 2]);

            return ret;
        }
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //NetworkStruct Generator

        //SendData To Clients
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        public void SetClientID(byte clientIDToSend)
        {
            


        }
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //SendData To Clients

    }
}

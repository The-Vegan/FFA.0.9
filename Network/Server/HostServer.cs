﻿using FFA.Empty.Empty.Network.Client;
using Godot;
using System;
using System.Net.Sockets;

namespace FFA.Empty.Empty.Network.Server
{
    public class HostServer
    {
        private BaseServer server;

        private PlayerInfo[] players = new PlayerInfo[16];

        public HostServer()
        {
            server = new BaseServer();

            server.ClientConnectedEvent += Connected;
            server.ClientDisconnectedEvent += Disconnected;
            server.DataRecievedEvent += DataRecieved;

        }
        //Packet Constants
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        /*CLIENT -> SERVER*/
        private const byte PING = 0;
        private const byte MOVE = 1;
        private const byte SET_CHARACTER = 2;
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
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Packet Constants
        public void Terminate()
        {
            server.Terminate();
        }

        private void DataRecieved(object sender, byte[] data, NetworkStream stream)
        {
            try
            {
                if (server.GetStream((byte)(data[1] - 1)) != stream) GD.Print("[HostServer] Stream doesn't match with corresponding ID : " + data[1]);
                GD.Print("[HostServer] Recieved data from client " + data[1]);
                switch (data[0])
                {
                    case PING:
                        GD.Print("[HostServer] PING recieved from " + data[1]);
                        server.GetStream((byte)(data[1] - 1)).Write(data, 0, data.Length);
                        break;
                    case MOVE:
                        GD.Print("[HostServer] MOVE recieved");
                        break;
                    case SET_CHARACTER:
                        byte id = data[1];
                        if (players[id - 1] == null)
                        {
                            GD.Print("[HostServer] Client is null server side");
                            break;
                        }
                        players[id - 1] = new PlayerInfo(data, 1);
                        UpdateNameList();
                        break;
                    default:
                        GD.Print("[HostServer] Unkown instruction : " + data[0]);
                        break;

                }
            }
            catch(Exception e)
            {
                GD.Print("[HostServer][Datarecieved] Incoherent data, threw exception : " + e);
            }

        }

        private void UpdateNameList()
        {
            byte[] output = PlayerInfo.SerialiseInfoArray(players);
            server.SendDataOnAllStreams(output);
        }

        private void Disconnected(object sender, byte clientID)
        {
            GD.Print("[HostServer] Client " + clientID + " Disconnected");
            players[clientID - 1] = null;
            UpdateNameList();
            GC.Collect();

        }

        private void Connected(object sender, byte id)
        {
            GD.Print("[HostServer] Client Connected on Slot " + id);
            byte[] output = new byte[8_192];
            output[0] = SET_CLIENT_OR_ENTITY_ID;
            output[1] = id;

            players[id - 1] = new PlayerInfo();
            players[id - 1].clientID = id;
            players[id - 1].characterID = 0;

            server.SendDataOnSingleStream(output, id);
            UpdateNameList();
        }
    }
}

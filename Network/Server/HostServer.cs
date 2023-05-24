using FFA.Empty.Empty.Network.Client;
using Godot;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FFA.Empty.Empty.Network.Server
{

    public class HostServer
    {
        private BaseServer server;

        private PlayerInfo[] players = new PlayerInfo[16];

        public delegate void LaunchIsAborted(object sender);
        public event LaunchIsAborted AbortingLaunch = delegate { };

        public delegate void CountDownSuccessfull(HostServer sender,bool success);
        public event CountDownSuccessfull CountdownWithoutEvents = delegate { };


        public PlayerInfo[] GetPlayer() { return players; }

        private bool launchAborted = true;

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
            launchAborted = true;
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
        //Events
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        private void Disconnected(object sender, byte clientID)
        {
            launchAborted = true;

            GD.Print("[HostServer] Client " + clientID + " Disconnected");
            players[clientID - 1] = null;
            UpdateNameList();
            GC.Collect();

        }

        private void Connected(object sender, byte id)
        {
            launchAborted = true;

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
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Event


        //Launch methods
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public void BeginLaunch()
        {
            byte[] signal = new byte[8_192];
            signal[0] = ABOUT_TO_LAUNCH;
            server.SendDataOnAllStreams(signal);
            launchAborted = false;
            for (sbyte sec = 10; sec >= 0; sec--)
            {             
                
                System.Threading.Thread.Sleep(250); if (launchAborted) break; 
                System.Threading.Thread.Sleep(250); if (launchAborted) break; 
                System.Threading.Thread.Sleep(250); if (launchAborted) break;
                System.Threading.Thread.Sleep(250); if (launchAborted) break;
                if (sec == 0) CountdownWithoutEvents(this,true);
                
            }//Launch might be aborted in another thread
            AbortLaunch();
        }

        public void AbortLaunch()
        {
            AbortingLaunch(this);
            byte[] outStream = new byte[8192];
            outStream[0] = ABORT_LAUNCH;
            server.SendDataOnAllStreams(outStream);
        }

        public void CompleteLaunch(byte mode)//Called from Level class
        {
            byte[] stream = new byte[8_192];
            stream[0] = LAUNCH;
            //Mode confirm
            stream[1] = mode;
            //MapID
            //If invalid, sends map as byte stream
            //players starting position
        }

        public void LaunchTimer()
        {
            byte[] stream = new byte[8_192];
            stream[0] = LAUNCH;
            server.SendDataOnAllStreams(stream);
        }


        public void SendStartSignalToAllClients(Dictionary<byte, Vector2> IDToEntity,byte lvlID)
        {
            byte[] stream = new byte[8_192];
            stream[0] = LAUNCH;
            stream[1] = lvlID;
            for(byte i = 0; i < players.Length; i++)
            {
                ushort offset = (ushort)(i * 5);

                stream[offset + 2] = players[i].clientID;

                Vector2 pos = IDToEntity[players[i].clientID];

                stream[offset + 3] = (byte)(((short)(pos.x) >> 8));
                stream[offset + 4] = (byte)(pos.x);
                stream[offset + 5] = (byte)(((short)(pos.y) >> 8));
                stream[offset + 6] = (byte)(pos.y);
            }

            server.SendDataOnAllStreams(stream);

        }

        internal void AssignRandomCharacters()
        {
            Random r = new Random();
            for (byte i = 0; i < players.Length; i++)
            {
                if (players[i] == null) continue;
                if (players[i].characterID == 0 || players[i].characterID > 3) players[i].characterID = (byte)r.Next(1, 4);
            }
            UpdateNameList();
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Launch methods
    }
}

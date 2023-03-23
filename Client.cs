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
        public byte clientID = 0;
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

        private EasyTcpClient client;

        private ClientData[] allClients = new ClientData[16];
        public ClientData[] GetClientData() { return allClients; }
        private Level map;
        private MainMenu menu;

        //Init
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        public Client(string connexionIP)
        {
            client = new EasyTcpClient();

            client.OnDataReceive += DataRecieved;
            client.OnDisconnect += Disconnected;
            client.OnConnect += Connected;
            client.OnError += InternalClientError;

            if (!client.Connect(connexionIP, 1404)) throw new ArgumentException("could not connect to server");
        }
        public void SetParent(MainMenu mm)
        {
            menu = mm ?? throw new ArgumentException("Can't set null menu in client");
            map = null;
            mm.multiplayer = true;
        }
        public void SetParent(Level lvl)
        {
            map = lvl ?? throw new ArgumentException("Can't set null level in client");
            menu = null;
        }
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Init
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
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Event Methods

        //Data IN
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        private void DataRecieved(object sender, Message e)
        {
            byte[] data = e.Data;

            switch (data[0])
            {
                //<<Launch
                case ABOUT_TO_LAUNCH: 
                    throw new NotImplementedException();
                case ABORT_LAUNCH: 
                    throw new NotImplementedException();
                case LAUNCH: 
                    throw new NotImplementedException();
                //Launch>>
                //<<Config Entities/Clients
                case SET_CLIENT_OR_ENTITY_ID:
                    clientID = data[1];
                    break;
                case SEND_CLIENT_LIST:
                    allClients = new ClientData[16];//Resets array

                    byte numberOfClientRecieved = data[1];
                    ushort offset = 2;
                    for (byte i = 0; i < numberOfClientRecieved; i++)
                    {
                        if (data[offset] > 16) continue;
                        //Extract basic data
                        ClientData cd = new ClientData()
                        {
                            clientID = data[offset],
                            characterID = data[offset + 1],
                            team = data[offset + 2],
                        };
                        offset += 3;
                        byte stringLength = data[offset];
                        offset++;
                        //Extract name
                        string nametag = Encoding.Unicode.GetString(data, offset, stringLength);
                        offset += stringLength;
                        cd.name = nametag;
                        //Adds client to list
                        allClients[cd.clientID - 1] = cd;
                    }
                    break;
                case SET_LEVEL_CONFIG: 
                    throw new NotImplementedException();
                //Config Entities/Clients>>
                //<<Game work
                case GAME_OVER:
                    throw new NotImplementedException();
                case GAME_SOON_OVER: 
                    throw new NotImplementedException();
                case SET_MOVES: 
                    throw new NotImplementedException();
                case SYNC: 
                    throw new NotImplementedException();
                case ITEM_GIVEN_BY_SERVER: 
                    throw new NotImplementedException();
                case BLUNDERED_BY_SERVER: 
                    throw new NotImplementedException();
                    //Game work>>
            }

        }

        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Data IN
        //Data OUT
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        public bool SendClientCharacterAndName(byte characterID,string name)
        {
            //Handles overflow
            if (name.Length > 32) return false;
            //creates a list with the basic informations
            List<byte> outstream = new List<byte>() { SET_CHARACTER , clientID ,characterID};
            //Adds the name to the byte stream
            byte[] nameAsByte = Encoding.Unicode.GetBytes(name);
            outstream.Add((byte)nameAsByte.Length);
            foreach (byte b in nameAsByte) outstream.Add(b);
            //sends
            try { client.FireOnDataSend(outstream.ToArray()); }
            catch (Exception)
            {
                GD.Print("[Client] Err, couldn't send character to server");
                return false;
            }
            //Confirm success
            return true;
        }
        public bool SendTeam(byte team)
        {
            try { client.FireOnDataSend(new byte[] { CHOSE_TEAM, clientID, team }); }
            catch (Exception)
            {
                GD.Print("[Client] Err, couldn't send team to server");
                return false;
            }
            return true;
        }
        public void SendMove(ushort packet)
        {

            for(byte i = 0; i < 128; i++)
            {
                try
                {
                    client.FireOnDataSend(new byte[] { MOVE,clientID,(byte)(packet >> 8),(byte)packet });
                    return;
                }
                catch (Exception)
                {
                    continue;//Retries if send failed
                }
            }
        }
        
        //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
        //Data OUT
    }
}

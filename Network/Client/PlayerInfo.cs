﻿using Godot;
using System;
using System.Text;


namespace FFA.Empty.Empty.Network.Client
{
    public class PlayerInfo
    {
        public PlayerInfo() { }
        public PlayerInfo(byte[] array, int offset)
        {
            this.clientID = array[offset];
            this.characterID = array[offset + 1];
            this.team = array[offset + 2];
            byte nameLength = array[offset + 3];

            if (nameLength != 0)
            {
                string name = Encoding.Unicode.GetString(array, (offset + 4), nameLength * 2);
                this.name = name;
            }

        }

        public byte clientID;
        public byte characterID;
        public byte team;
        public string name;

        public byte[] ToByte()
        {
            byte[] stream;
            if (this.name == null)
            {
                stream = new byte[3];
                stream[0] = this.clientID;
                stream[1] = this.characterID;
                stream[2] = this.team;
                stream[3] = 0;
            }
            else
            {
                stream = new byte[3 + (this.name.Length * 2)];
                stream[0] = this.clientID;
                stream[1] = this.characterID;
                stream[2] = this.team;
                stream[3] = (byte)this.name.Length;
                byte[] nameAsByte = Encoding.Unicode.GetBytes(name);
                for (byte i = 0; i < nameAsByte.Length; i++) stream[3 + i] = nameAsByte[i];
            }

            return stream;
        }

        public override string ToString()
        {
            if (this.name == null || this.name == "")
                return this.clientID + "\t: " + this.characterID + "\t: " + team + "\t: Unnamed";
            return this.clientID + "\t: " + this.characterID + "\t: " + team + "\t: " + this.name;
        }

        public static byte[] SerialiseInfoArray(PlayerInfo[] playerList)
        {
            byte[] output = new byte[8_192];
            ushort offset = 2;
            output[0] = 251;//SEND_NAME_LIST

            for (byte i = 0; i < playerList.Length; i++)
            {
                if (playerList[i] == null) continue;
                else output[1]++;

                byte[] plyrAsByte = playerList[i].ToByte();

                for (ushort j = 0; j < plyrAsByte.Length; j++) output[offset + j] = plyrAsByte[j];

                offset += (ushort)plyrAsByte.Length;

            }
            GD.Print("[PlayerInfo] Server found " + output[1] + " clients in list");
            return output;
        }

        public static PlayerInfo[] DeserialiseInfoArray(byte[] data)
        {
            byte nmbrOfPlayer = data[1];

            if (nmbrOfPlayer > 16 || nmbrOfPlayer == 0) throw new ArgumentException("incorrect number of players");

            PlayerInfo[] retArray = new PlayerInfo[16];
            ushort offset = 2;
            for (byte i = 0; i < nmbrOfPlayer; i++)
            {
                PlayerInfo player = new PlayerInfo(data, offset);
                if (player.name == null) offset += 3;
                else offset += (ushort)(3 + (player.name.Length * 2));

                retArray[player.clientID - 1] = player;
            }

            return retArray;
        }

    }
}

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA.Empty.Empty
{
    public class ClientData
    {
        public ClientData() { }

        public byte clientID;
        public byte characterID;
        public byte team;
        public string name;

        public byte[] ToBytes()
        {
            byte[] nameAsByte = Encoding.Unicode.GetBytes(name);

            byte[] ret = new byte[nameAsByte.Length + 4];
            ret[0] = clientID;
            ret[1] = characterID;
            ret[2] = team;
            ret[3] = (byte)nameAsByte.Length;

            for (byte i = 0; i < ret.Length; i++)
            {
                ret[4 + i] = nameAsByte[i];
            }

            return ret;
        }
    }

    public class EntitySync
    {
        public EntitySync() { }

        //statistics
        public short health;
        public byte itemBar;
        public byte blunderBar;
        public byte heldItem;
        public Vector2 coordinate;
        //status effects
        public byte stun;
        public bool blundered;
        public byte cooldown;

        public byte[] ToBytes()
        {
            byte[] ret = new byte[10];

            ret[0] = (byte)(health >> 8);
            ret[1] = (byte) health;
            ret[2] = itemBar;
            ret[3] = blunderBar;
            ret[4] = heldItem;
            ret[5] = (byte)coordinate.x;
            ret[6] = (byte)coordinate.y;
            ret[7] = stun;
            if (blundered) ret[8] = 255;
            ret[9] = cooldown;

            return ret;
        }
    }

    public class DamageTileSync
    {
        public DamageTileSync() { }

        public byte sourceID;
        public short damage;
        
        public byte[] ToBytes() { return new byte[] { sourceID, (byte)(damage >> 8), (byte)damage }; }

    }



}

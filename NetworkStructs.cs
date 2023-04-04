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

        public ClientData ClientDataFromBytes(byte[] serializedClientData,int offset) 
        {
            ClientData c = new ClientData();

            c.clientID = serializedClientData[offset];
            c.characterID = serializedClientData[offset + 1];
            c.team = serializedClientData[offset + 2];

            c.name = Encoding.Unicode.GetString(serializedClientData, (offset + 4), serializedClientData[offset + 3]);


            return c;
        }

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

        public EntitySync ESyncFromBytes(byte[] serializedSyncData, int offset)
        {
            EntitySync es = new EntitySync();

            es.health = (short)((serializedSyncData[offset] << 8) + serializedSyncData[offset + 1]);
            es.itemBar = serializedSyncData[offset + 2];
            es.blunderBar = serializedSyncData[offset + 3];
            es.heldItem = serializedSyncData[offset + 4];

            es.coordinate = new Vector2(serializedSyncData[offset + 5], serializedSyncData[offset + 6]);

            es.stun = serializedSyncData[offset + 7];
            es.blundered = (serializedSyncData[offset+8] != 0);
            es.cooldown = serializedSyncData[offset+9];

            return es;
        }
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
        public DamageTileSync DamageTileSyncFromBytes(byte[] data, int offset) 
        {
            DamageTileSync ret = new DamageTileSync();
            ret.sourceID = data[offset];
            ret.damage = (short)((data[offset+1] << 8) + data[offset + 2]);

            return ret;
        }
        public byte[] ToBytes() { return new byte[] { sourceID, (byte)(damage >> 8), (byte)damage }; }

    }



}

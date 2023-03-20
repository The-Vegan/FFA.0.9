using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA.Empty.Empty
{
    public struct ClientData
    {
        public byte clientID;
        public byte characterID;
        public byte team;
        public string name;
    }

    public struct EntitySync
    {
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
    }

    public struct DamageTileSync
    {
        public byte sourceID;
        public short damage;
    }

    public struct SyncPacketFormat
    {
        public Dictionary<byte, EntitySync> idToEntity;
        public Dictionary<Vector2, DamageTileSync> coordsToAtk;
    }

    


}

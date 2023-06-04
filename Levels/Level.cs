using FFA.Empty.Empty.Network.Client;
using FFA.Empty.Empty.Network.Server;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public abstract class Level : TileMap
{
    //Level Variable
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected short globalBeat = 0;
    protected Random rand = new Random();
    protected Vector2[] spawnpoints = new Vector2[12];

    protected List<Vector2[]> TeamSpawnPoints = new List<Vector2[]>();

    protected Timer timer;
    protected Camera2D camera;
    protected PackedScene hudScene = GD.Load<PackedScene>("res://UIAndMenus/HUD/Hud.tscn");

    protected bool teamMode = false;

    public abstract byte GetLvlID();
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Level Variable

    //Network
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected LocalClient client;
    protected HostServer server;

    protected Dictionary<byte,NetworkController> distantPlayerControllers = new Dictionary<byte, NetworkController>();
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Network

    //Entities Variables
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected byte idToGive = 0;
    protected List<Entity> allEntities = new List<Entity>();
    protected Dictionary<byte, Entity> idToEntity = new Dictionary<byte, Entity>();
    protected Dictionary<Vector2, Entity> coordToEntity = new Dictionary<Vector2, Entity>();
    protected Dictionary<Vector2, Entity> oldCoordToEntity = new Dictionary<Vector2, Entity>();
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Entities Variables

    //DEPENDANCIES
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\  
    protected Entity mainPlayer;
    
    protected PackedScene atkScene = GD.Load("res://Abstract/Attack.tscn") as PackedScene;

    [Signal]
    protected delegate void checkEndingCondition();

    protected PackedScene pirateScene = GD.Load("res://Entities/Pirate/Pirate.tscn") as PackedScene;
    protected PackedScene blahajScene = GD.Load("res://Entities/Blahaj/Blahaj.tscn") as PackedScene;
    protected PackedScene monstropisScene = GD.Load("res://Entities/Monstropis/Monstropis.tscn") as PackedScene;

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //DEPENDANCIES


    //INIT METHODS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public bool InitPlayerAndMode(byte chosenCharacter, byte gameMode, byte numberOfTeams)//Solo
    {
        PackedScene controllScene = GD.Load("res://Abstract/ControllerPlayer.tscn") as PackedScene;

        InitGameMode(gameMode, numberOfTeams);

        mainPlayer = CreateEntityInstance(chosenCharacter,controllScene, "bob");//CreateEntityInstance Adds the entity to the List of all entities

        //Loads Controller outside of loop
        PackedScene CPU = GD.Load<PackedScene>("res://Abstract/CPUController.tscn");
        for (byte i = 1; i < 16; i++)
        {
            CreateEntityInstance(CPU,"bob");
        }
        GD.Print("[Level] LoadCompleted in level");
        return true;

    }
    
    public Dictionary<byte,Vector2> InitPlayerAndModeMulti(byte gameMode,byte numberOfTeams)//multi
    {
        if (server == null) return null;

        PlayerInfo[] players = server.GetPlayer();
        if (players.Length > 16) throw new ArgumentException("[Level] Invalid PlayerInfo Array");

            Dictionary<byte, Vector2> IDToEntity = new Dictionary<byte, Vector2>();
        InitGameMode(gameMode, numberOfTeams);

        PackedScene controllerToLoad = GD.Load<PackedScene>("res://Abstract/NetworkController.tscn");

        Entity entity;        

        for (byte i = 0;i < players.Length; i++)
        {
            if (players[i] == null) continue;
            if (players[i].clientID == client.clientID)//If the client being loaded is the local client, loads a different controller
            {
                PackedScene controllScene = GD.Load("res://Abstract/ControllerPlayer.tscn") as PackedScene;
                entity = CreateEntityInstance(players[i].characterID, controllScene, players[i].name, players[i].clientID);
                mainPlayer = entity;
            }
            else//If the player is distant, use the networkController
            {
                entity = CreateEntityInstance(players[i].characterID, controllerToLoad, players[i].name, players[i].clientID);
            }
            
            Spawn(entity);

            IDToEntity.Add((byte)(i + 1), entity.pos);

        }
        return IDToEntity;
    }

    internal void SendPacket(short p)
    {
        client?.SendPacketToServer(p, this.timer.TimeLeft);
    }

    public void AddNetworkController(NetworkController c,byte id)
    {
        distantPlayerControllers.Add(id, c);
    }

    public void InitPlayerAndModeClient()
    {
        if (server != null) return;

        PlayerInfo[] players = client.GetPlayersInfo();
        PackedScene networkController = GD.Load<PackedScene>("res://Abstract/NetworkController.tscn");
        for(byte i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            if (players[i].clientID == client.clientID)
            {
                PackedScene controllScene = GD.Load("res://Abstract/ControllerPlayer.tscn") as PackedScene;
                mainPlayer = CreateEntityInstance(players[i].characterID, controllScene, players[i].name, players[i].clientID);
                Spawn(mainPlayer);
            }
            else Spawn(CreateEntityInstance(players[i].characterID, networkController, players[i].name, players[i].clientID));
        }


    }

    public void InitNetwork(HostServer ser,LocalClient cli) { this.server = ser; server.map = this; this.client = cli; client.SetParent(this);}
    public void InitNetwork(LocalClient cli) { this.client = cli; client.SetParent(this); }
    protected void InitGameMode(byte gameMode, byte numberOfTeams)
    {
        switch (gameMode)//TODO : code the modes
        {
            case 0:
            default://fail-safe
                this.Connect("checkEndingCondition", this, "ClassicEndCond");
                teamMode = false;
                InitSpawnPointsClasssic();
                GD.Print("[Level] Classic");
                break;
            case 1:
                this.Connect("checkEndingCondition", this, "TeamEndCond");
                teamMode = true;
                InitSpawnPointsTeam(numberOfTeams);
                GD.Print("[Level] Team");
                break;
            case 2:
                this.Connect("checkEndingCondition", this, "CTFEndCond");//CTF NOT CODED
                teamMode = true;
                InitSpawnPointsCTF(numberOfTeams);
                GD.Print("[Level] CTF");
                break;
            case 3:
                this.Connect("checkEndingCondition", this, "SiegeEndCond");//SACKING NOT CODED
                teamMode = true;
                InitSpawnPointsSiege();
                GD.Print("[Level] Siege");
                break;
        }
    }

    //OVERRIDE
    protected abstract void InitSpawnPointsClasssic();
    protected abstract void InitSpawnPointsTeam(int nbrOfTeams);
    protected abstract void InitSpawnPointsCTF(int nbrOfTeams);
    protected abstract void InitSpawnPointsSiege();//Always 4 teams
    //OVERRIDE

    public override void _Ready()
    {
        camera = this.GetNode("Camera2D") as Camera2D;
        timer = this.GetNode<Timer>("Timer");
        this.RemoveChild(camera);
        
        Hud hud = hudScene.Instance() as Hud;

        GD.Print("[Level] Ready , camera = " + camera + " : timer = " + timer + " : hud" + hud);
        
        mainPlayer.AddChild(hud,true);
        mainPlayer.Connect("noteHiter", hud, "HitNote");

        mainPlayer.AddChild(camera,true);
        

        camera.Current = true;

        if (client == null) timer.Start();
    }
    public void StartTimer() { timer.Start(); }
    public float GetTime()
    {
        return timer.TimeLeft;
    }

    protected void SpawnAllEntities(byte numberOfEntities)
    {
        PackedScene cpu = GD.Load("res://Abstract/GenericController.tscn") as PackedScene;
        for (int i = allEntities.Count; i < numberOfEntities; i++)
        {
           CreateEntityInstance(cpu,"bob");
        }
        for (int i = 0; i < allEntities.Count; i++)
        {
            Spawn(allEntities[i]);
        }

    }

    public async void InitPlayerCoordinates(Dictionary<byte, Vector2> IDToCoords)
    {
        if (!this.IsInsideTree()) await ToSignal(this, "ready");

        byte[] keys = IDToCoords.Keys.ToArray();
        GD.Print("[Level] Setting Entity positions");
        for (byte i = 0; i < keys.Length; i++) GD.Print(keys[i] + " : " + IDToCoords[keys[i]]);
        for (byte i = 0; i < keys.Length; i++) allEntities[keys[i]-1].Moved(IDToCoords[keys[i]]);
        for(byte i = 0; i < keys.Length; i++) allEntities[keys[i]-1].Moved(IDToCoords[keys[i]]);//BugFix

    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //INIT METHODS

    //ENTITY RELATED METHODS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Entity CreateEntityInstance(PackedScene pcs,string nametag)
    {
        return CreateEntityInstance(rand.Next(1,4), pcs, nametag);//Creates random entity
    }
    protected Entity CreateEntityInstance(int entityID,PackedScene controllScene,string nametag)//solo
    {
        Entity playerEntity;
        //Selects correct entity from parameter ID
        switch (entityID)
        {
            case 1://Pirate
                GD.Print("[Level] Make a pirate");
                playerEntity = pirateScene.Instance() as Pirate;       
                break;
            case 2://♥
                GD.Print("[Level] Make a ♥");
                playerEntity = blahajScene.Instance() as Blahaj;
                break;
            case 3:
                GD.Print("[Level] Make a monstropis");
                playerEntity = monstropisScene.Instance() as Monstropis;
                break;

            default://Random
                return CreateEntityInstance(rand.Next(1, 4), controllScene,nametag);

        }//End of characters switch statement

        //Finalizes configurations for player entity
        allEntities.Add(playerEntity);
        GD.Print("[Level] Added Entity to the tree : " + playerEntity);
        do 
        {
            if (idToEntity.Count >= 250) throw new OverflowException("How did you even summon 250+ entities??? ;-;");

            idToGive++;
            try
            {
                idToEntity.Add(idToGive, playerEntity);
            }
            catch (ArgumentException)//Give an ID and adds it in dictionary
            {
                continue;
            }
        
        } while (false);

        playerEntity.Init(this, controllScene, nametag, idToGive);

        this.AddChild(playerEntity, true);
        GD.Print("[Level] Entity initialized and added to tree");


        return playerEntity;
    }
    protected Entity CreateEntityInstance(int entityID, PackedScene controllScene, string nametag, byte clientID)
    {
        if (clientID == 0 || clientID > 16) throw new ArgumentException("ClientID must be between 1 and 16");
        Entity playerEntity;
        //Selects correct entity from parameter ID
        switch (entityID)
        {
            case 1://Pirate
                GD.Print("[Level] Make a pirate");
                playerEntity = pirateScene.Instance() as Pirate;
                break;
            case 2://♥
                GD.Print("[Level] Make a ♥");
                playerEntity = blahajScene.Instance() as Blahaj;
                break;
            case 3:
                GD.Print("[Level] Make a monstropis");
                playerEntity = monstropisScene.Instance() as Monstropis;
                break;
            default://Random
                return CreateEntityInstance(rand.Next(1, 4), controllScene, nametag, clientID);

        }//End of characters switch statement

        //Finalizes configurations for player entity
        allEntities.Add(playerEntity);
        
        idToEntity.Add(clientID, playerEntity);
        playerEntity.Init(this, controllScene, nametag, clientID);
        this.AddChild(playerEntity, true);
        GD.Print("[Level] Entity initialized and added to tree");
        return playerEntity;
    }
    public void DeleteEntity(Entity entity)
    {
        byte eID = entity.id;
        idToEntity.Remove(entity.id);
        allEntities.Remove(entity);

        entity.QueueFree();
        GD.Print("{[Level] " + entity + " has been deleted from existance");
    }

    public void MoveEntity(Entity entity,Vector2 newTile)
    {
        //Checks if tile is walkable
        if (this.GetCell((int)(newTile.x),(int)(newTile.y)) == 0)
        {
            entity.Moved(newTile);
        }
        else
        {
            entity.Moved(entity.pos);
        }
    }

    public async void Spawn(Entity entity)
    {
        if (!this.IsInsideTree())
        {
            GD.Print("[Level] Level outside of tree, awaiting ready");
            await ToSignal(this, "ready");
            GD.Print("[Level] Ready Signal Recieved");
        }


        byte failures = 0;//Forces spawning if fails too much

        entity.ResetHealth();
        GD.Print("[Level] Spawning " + entity);
        while (failures < 65)
        {
            if (!teamMode)
            {
                int randomTile = rand.Next(spawnpoints.Length);

                if (SpawnPointInoccupied(randomTile))//If tile isn't occupied
                {
                    entity.Moved(spawnpoints[randomTile]);
                    await ToSignal(entity.GetNode("Tween"), "tween_completed");
                    entity.Visible = true;
                    break;
                }
            }
            else //if (teamMode)
            {
                int randomTile = rand.Next(TeamSpawnPoints[entity.team].Length);

                if ((this.GetCell((int)TeamSpawnPoints[entity.team][randomTile].x, (int)TeamSpawnPoints[entity.team][randomTile].y) == 0) && (failures < 64))
                {
                    entity.Moved(TeamSpawnPoints[entity.team][randomTile]);
                    await ToSignal(entity.GetNode("Tween"), "tween_completed");
                    entity.Visible = true;
                    break;
                }
            }
            failures++;
        }
       
    }

    private bool SpawnPointInoccupied(int randomTile)
    {
        Vector2 tile = spawnpoints[randomTile];
        if ((this.GetCell((int)tile.x, (int)tile.y) != 0)) return false;

        for (short yAxis = (short)(tile.y - 2); yAxis <= (short)(tile.y + 2); yAxis++)
        {
            for (short xAxis = (short)(tile.x - 2); xAxis <= (short)(tile.x + 2); xAxis++)
            {
                if (this.GetCell(xAxis, yAxis) == 3) return false;
            }
        }
        return true;
    }

    public void SetEntityPacket(byte entityID, short packet, float timing)
    {
        if (!idToEntity.ContainsKey(entityID))
        {
            GD.Print("[Level] ERROR : Entity "+ entityID +" not found");
            return;
        }

        Entity entity = idToEntity[entityID];
        entity.SetPacket(packet);

        float delta = Math.Abs(entity.timing - timing);//Obtient l'écart entre lvl et entité

        if (delta < 0.03f) entity.timing = timing;//Cecks for lag(in seconds)
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ENTITY RELATED METHODS

    //ATTACK RELATED METHODS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void CreateAtk(Entity source, List<List<Dictionary<String, short>>> atkData, String path, byte[] collumns, bool flipable)
    {
        Attack atkInstance = atkScene.Instance() as Attack;
        atkInstance.InitAtk(source, atkData, this, path, collumns, flipable);
        this.AddChild(atkInstance, true);
    }

    public void DamageEntity(Attack atk)
    {
        int damageTiles = atk.GetChildCount();
        for(int i = 0; i < damageTiles; i++)//Goes through all the damagetiles in the Attack
        {
            DamageTile dt = atk.GetChild<DamageTile>(i);

            if (dt.GetDamage() <= 0) continue;
            try
            {
                coordToEntity[dt.GetCoords()].Damaged(atk.GetSource(),dt.GetDamage());//Finds entity on tile and damages it
                oldCoordToEntity[dt.GetCoords()].Damaged(atk.GetSource(),dt.GetDamage());
            }
            catch (KeyNotFoundException)//No entities on that tile
            {
                continue;
            }
        }
    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ATTACK RELATED METHODS

    //ENDING CONDITION
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\

    protected void ClosingArena()
    {
        GD.Print("[Level] Hello World");
    }

    protected virtual void ClassicEndCond()
    {
        if(globalBeat > 200)
        {
            ClosingArena();
        }
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ENDING CONDITION

    //NETWORKING
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void ResyncEntities(List<SyncEntityPacket> allPackets)
    {
        for(byte i = 0; i < allPackets.Count; i++)
        {
            Entity resync = idToEntity[allPackets[i].entityID];

            resync.Sync(allPackets[i]);
        }
    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //NETWORKING
    public void TimerUpdate()
    {
        globalBeat++;
        //GD.Print("[Level] - - - - - - - - - - - - - - - - - - - - - - - - " + globalBeat);
        if(server != null)
        {
            server.SendSyncPosition(allEntities);
            server.SendAllMovePackets(allEntities);
        }

        UpdateAllEntities();

        UpdatePositionDictionary();

        GetTree().CallGroup("Attacks", "BeatAtkUpdate");//Also updates the hud

        EmitSignal("checkEndingCondition");
    }
    public void TimerUpdate(LocalClient sender)
    {
        if (server != null) return;

        globalBeat++;
        //GD.Print("[Level] - - - - - - - - - - - - - - - - - - - - - - - - " + globalBeat);

        UpdateAllEntities();

        UpdatePositionDictionary();

        GetTree().CallGroup("Attacks", "BeatAtkUpdate");//Also updates the hud

        EmitSignal("checkEndingCondition");
    }

    protected void UpdateAllEntities()
    {
        SortAllEntities();

        for (int i = 0;i < allEntities.Count; i++)
        {
            allEntities[i].BeatUpdate();
        }
    }

    protected void SortAllEntities()//CombSort
    {
        int gap = allEntities.Count >> 1;

        while(gap != 0)
        {
            Entity tempEntity;

            for(int i = 0;i < allEntities.Count - gap; i++)
            {
                
                if(Math.Abs(allEntities[i].timing - (1f / 6f)) > Math.Abs( allEntities[i + gap].timing - (1f / 6f)))//If entity in front has bigger score(worse)
                {
                    //Swap
                    tempEntity = allEntities[i];
                    allEntities[i] = allEntities[i + gap];
                    allEntities[i + gap] = tempEntity;
                }
            }
            gap--;
        }
    }

    private void UpdatePositionDictionary()
    {
        coordToEntity = new Dictionary<Vector2, Entity>();
        oldCoordToEntity = new Dictionary<Vector2, Entity>();
        
        for (int i= 0; i < allEntities.Count; i++)
        {
            try
            {
                coordToEntity.Add(allEntities[i].pos, allEntities[i]);//Adds the position of the entity as it's key
            }
            catch (ArgumentException) { }
            try
            {
                oldCoordToEntity.Add(allEntities[i].prevPos, allEntities[i]);//Separate dictionaries to avoid problems
            }
            catch (ArgumentException) { continue; }
        }
    }



}

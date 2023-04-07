
using FFA.Empty.Empty;
using Godot;
using System;
using System.Collections.Generic;
using System.Net;

public class MainMenu : Control
{
    //Nodes
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Camera2D camera;
    protected VBoxContainer playerListBox;

    protected Label ipLineEditMessage;

    public LineEdit nameBox;
    public Sprite resetNetworkConfigForm;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Nodes

    //Level Initialisation Variables
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public byte gameMode = 0;
    public byte playerCharacter = 0;
    public byte numberOfTeams = 1;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Level Initialisation Variables

    //Network
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    private Server server;
    private Client client;

    public bool multiplayer = false;
    public bool hosting = false;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Network

    public override void _Ready()
    {
        camera = this.GetNode("Camera2D") as Camera2D;

        playerListBox = this.GetNode("WaitForPlayers/VBoxContainer") as VBoxContainer;
        ipLineEditMessage = this.GetNode("ConnectToServer/IpTextBox/Label") as Label;
        nameBox = GetNode("Camera2D/CanvasLayer/NameBox") as LineEdit;

        resetNetworkConfigForm = GetNode("Camera2D/CanvasLayer/ResetNetworkConfigForm") as Sprite;
    }

    //Camera Position
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected List<Vector2> back = new List<Vector2>() { new Vector2(0, 0) };

    protected Vector2 MAINMENU = new Vector2(0, 0);
    protected Vector2 SOLO = new Vector2(0, -576);
    protected Vector2 CHARSELECT = new Vector2(-1024, 0);
    protected Vector2 LEVELSELECT = new Vector2(-2048, 0);
    protected Vector2 MULTI = new Vector2(0, 576);
    protected Vector2 JOIN = new Vector2(-1024, 576);
    protected Vector2 WAITFOROTHERS = new Vector2(-3072, 0);

    public void MoveCameraTo(sbyte destination)
    {
        if(destination == -1)
        {
            camera.Position = back[back.Count - 1];
            back.RemoveAt(back.Count - 1);

            if (back.Count <= 0) back.Add(MAINMENU);
            return;
        }

        back.Add(camera.Position);
        nameBox.Visible = false;
        switch (destination)
        {
            case 0://MainMenu                                   
                ResetNetworkConfigAndGoBackToMainMenu();                          
                break;
            case 1://Solo
                ResetNetworkConfigAndGoBackToMainMenu();
                camera.Position = SOLO;
                break;
            case 2://Character Select
                camera.Position = CHARSELECT;
                if(client != null) nameBox.Visible = true;
                break;
            case 3://Level Select
                camera.Position = LEVELSELECT;
                break;
            case 4://Multi
                camera.Position = MULTI;
                break;
            case 5://Join
                camera.Position = JOIN;
                break;
            case 6://WaitForHostToStartGame
                camera.Position = WAITFOROTHERS;
                if (client != null) nameBox.Visible = true;
                break;
            default://Returns to MainMenu in case of error
                ResetNetworkConfigAndGoBackToMainMenu();
                break;
        }
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Camera Position

    //IHM
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void SetGame(byte mode)
    {
        this.gameMode = mode;
        GD.Print("[MainMenu] gameMode set to : " + gameMode);
    }

    public void SetCharacter(byte character)
    {
        playerCharacter = character;
        GD.Print("[MainMenu] playerCharacter set to : " + playerCharacter);
    }

    public void CharacterChosen()//Called from the "Next" button on character screen
    {
        if (server != null || client == null)//If this is localhost OR if this is solo
            MoveCameraTo(3);
        else MoveCameraTo(6);
    }

    public void DisplayPlayerList(ClientData[] playerList)
    {
        GD.Print("[MainMenu] playerListCount : " + playerList.Length);
        //Finds the label corresponding to players and sets thier theme to "connected"
        for (byte i = 0; i < playerList.Length; i++)
        {
            CheckButton playerLabel = playerListBox.GetChild<CheckButton>(playerList[i].clientID - 1);
            playerLabel.Pressed = true;
            playerLabel.Text = playerList[i].name;
            GD.Print("[MainMenu] changed button " + playerList[i].clientID);
        }
    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //IHM

    //Network Related
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public bool CreateServer()
    {
        try {this.server = new Server(); }
        catch (Exception) { return false; }
        return true;
    }

    public bool CreateClient(string serverIP)
    {
        try { this.client = new Client(serverIP); }
        catch (Exception) { return false; }
        return true;
    }

    public void ResetNetworkConfigAndGoBackToMainMenu()
    {
        if (this.server != null) 
        { 
            server.ShutDownServer(); 
            this.server = null;
        }
        if (this.client != null) 
        { 
            client.ShutDownClient();
            this.client = null;
        }
        GC.Collect();
        multiplayer = false;
        camera.Position = MAINMENU;
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Network Related

    //Level Loading
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void LoadLevel(PackedScene lvlToLoad)
    {
        if(client != null)
        {
            Level loadedLevel = lvlToLoad.Instance() as Level;

            loadedLevel.InitPlayerAndMode(client.GetClientData(), gameMode, numberOfTeams);

        }
        

       
    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Level Loading
}

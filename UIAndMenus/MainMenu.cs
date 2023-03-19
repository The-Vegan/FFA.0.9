
using Godot;
using System;
using System.Collections.Generic;
using System.Net;

public class MainMenu : Control
{
    //Nodes
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Camera2D camera;
    protected Label multiPlayerCounter;

    public byte postCharacterDestination = 3;

    public byte gameMode = 0;
    public byte playerCharacter = 0;
    public byte teams = 1;
    public byte chosenTeam = 0;
    public byte numberOfEntities = 12;
    public byte numberOfPlayers = 1;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Nodes

    //Network
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //private HostServer server;
    //private LocalClient client;

    public bool multiplayer = false;
    public bool hosting = false;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Network


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


    public override void _Ready()
    {
        camera = this.GetNode("Camera2D") as Camera2D;
        multiPlayerCounter = this.GetNode("WaitForPlayers/Label") as Label;
    }

    public void MoveCameraTo(sbyte destination)
    {
        if(destination == -1)
        {
            camera.Position = back[back.Count - 1];
            back.RemoveAt(back.Count - 1);

            if (back.Count == 0) back.Add(MAINMENU);
            return;
        }

        back.Add(camera.Position);

        switch (destination)
        {
            case 0://MainMenu                                   
                ResetNetworkConfig();                          
                camera.Position = MAINMENU;
                break;
            case 1://Solo
                ResetNetworkConfig();
                camera.Position = SOLO;
                break;
            case 2://Character Select
                camera.Position = CHARSELECT;
                break;
            case 3://Level Select
                camera.Position = LEVELSELECT;
                break;
            case 4://Multi
                ResetNetworkConfig();
                camera.Position = MULTI;
                break;
            case 5://Join
                camera.Position = JOIN;
                break;
            case 6://WaitForHostToStartGame
                camera.Position = WAITFOROTHERS;
                break;
            default://Returns to MainMenu in case of error
                ResetNetworkConfig();
                MoveCameraTo(0);
                break;

        }

    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Camera Position
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

    //Network Related
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void CreateServer()
    {
        throw new NotImplementedException();
    }

    public void CreateClient(string serverIP)
    {
        throw new NotImplementedException();
    }

    public void ResetNetworkConfig()
    {
        throw new NotImplementedException();
    }

    public void SetConnectedPlayers(byte connected)
    {
        multiPlayerCounter.Text = connected + "/" + numberOfPlayers + "  ";
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Network Related
}

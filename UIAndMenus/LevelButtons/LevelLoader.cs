using Godot;
using System;

public class LevelLoader : Button
{
    [Export]
    protected PackedScene lvlToLoad;
    protected MainMenu mainMenu;

    public override void _Ready()
    {
        mainMenu = GetParent().GetParent() as MainMenu;
    }

    public override void _Pressed()
    {
        if(mainMenu.multiplayer)
        {
            mainMenu.MoveCameraTo(6);
        }
        else
        {
            mainMenu.LoadLevel(lvlToLoad);
        }
    }
}

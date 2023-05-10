using Godot;

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
            GD.Print("[LevelLoader] multiplayer");
            mainMenu.MoveCameraTo(6);
        }
        else
        {
            GD.Print("[LevelLoader] Solo");
            mainMenu.LoadLevel(lvlToLoad);
        }
    }
}

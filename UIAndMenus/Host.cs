using Godot;
using System;

public class Host : Button
{
    protected MainMenu mm;
    public override void _Ready()
    {
        mm = this.GetParent().GetParent() as MainMenu;
    }

    public override void _Pressed()
    {
        mm.MoveCameraTo(1);
        mm.CreateServer();
        mm.postCharacterDestination = 6;
}
}

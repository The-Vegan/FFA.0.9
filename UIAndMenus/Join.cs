using Godot;
using System;

public class Join : Button
{
    protected MainMenu mm;
    public override void _Ready()
    {
        mm = this.GetParent().GetParent() as MainMenu;
    }

    public override void _Pressed()
    {
        mm.MoveCameraTo(5);
        
        mm.postCharacterDestination = 6;
    }
}

using Godot;
using System;
using System.Net;

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
        if (!mm.CreateClient(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString())) GD.Print("[Host] Failed to connect to localhost");
    }
}

using Godot;
using System;
using System.Text.RegularExpressions;

public class ConnectToServerButton : Button
{
    private MainMenu mm;
    private Container parent;
    private LineEdit ipTextBox;
    private Label msg;
    public override void _Ready()
    {
        parent = GetParent() as Container;
        mm = parent.GetParent() as MainMenu;
        ipTextBox = parent.GetNode("IpTextBox") as LineEdit;
        msg = ipTextBox.GetNode("Label") as Label;
    }
    public override void _Pressed()
    {

        Regex ipReg = new Regex("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");

        if (ipReg.IsMatch(ipTextBox.Text))
        {
            GD.Print("Regex passed");
            mm.CreateClient(ipTextBox.Text);

            if (/*Methode de connexion au serveur interne au client*/true)
            {
                mm.MoveCameraTo(2);//Sends to character select
            }
            else msg.SetText("can't find server");
        }
        else msg.SetText("invalid IP");
    }
}

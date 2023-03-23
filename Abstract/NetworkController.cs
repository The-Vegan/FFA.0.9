using FFA.Empty.Empty;
using Godot;
using System;

public class NetworkController : GenericController
{
    private Client client;
    public override void _Ready()
    { }
    public void Init(Client cli)
    {
        this.client = cli;
    }

}

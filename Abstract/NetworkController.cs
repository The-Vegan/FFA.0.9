using FFA.Empty.Empty.Network.Client;

public class NetworkController : GenericController
{
    private LocalClient client;
    public override void _Ready()
    { }
    public void Init(LocalClient cli)
    {
        this.client = cli;
    }

}

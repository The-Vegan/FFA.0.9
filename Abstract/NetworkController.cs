using FFA.Empty.Empty.Network.Client;

public class NetworkController : GenericController
{
    private LocalClient client;

    public void Init(LocalClient cli)
    {
        this.client = cli;
    }

    public void PacketSetByServer(short p)
    {
        entity.SetPacket(p);
    }

}

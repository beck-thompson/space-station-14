using System.Threading.Tasks;
using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.PlaytimeShare;

namespace Content.Server.PlaytimeShare;

public sealed class TimeTransferAdminEui : BaseEui
{
    [Dependency] private readonly PlaytimeShareManager _playtime = default!;

    // servers
    // playtime transfers
    private List<TimeTransferServerInfoSerializable> _trustedServers = new();

    public TimeTransferAdminEui()
    {
        IoCManager.InjectDependencies(this);
    }

    public override async void Opened()
    {
        // db query
        RefreshServerDbEntries();
        StateDirty();
    }

    public override EuiStateBase GetNewState()
    {
        return new TimeTransferAdminState
        {
            // use cached db values
            TrustedServers = _trustedServers,
            PrivateKeyStatus = _playtime.GetPrivateKeyStatus(),
            PublicKey = _playtime.GetPublicKey(),
        };
    }

    private async Task RefreshServerDbEntries()
    {
        var servers = await _playtime.GetAllTrustedServerInfo();
        _trustedServers = servers;
        StateDirty();
    }
}

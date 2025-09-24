using System.Text.Json;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.PlaytimeShare;

[Serializable, NetSerializable]
public sealed class TimeTransferAdminState: EuiStateBase
{
    public List<TimeTransferServerInfoSerializable> TrustedServers = new();

    public PrivateKeyStatus PrivateKeyStatus;
    public string PublicKey = "";
}

public enum PrivateKeyStatus
{
    Valid,
    Invalid,
    Empty,
}

// Basically 1:1 the DB entry
[Serializable, NetSerializable]
public sealed class TimeTransferServerInfoSerializable
{
    public string ServerPublicKey = string.Empty;

    public string ServerName = string.Empty;

    public bool Enabled;

    public bool AutoApproveTransfers;

    public TimeSpan ApplicationMaxAge;

    public Dictionary<string, string> RoleTransferData = new();

    public string Note = string.Empty;
}


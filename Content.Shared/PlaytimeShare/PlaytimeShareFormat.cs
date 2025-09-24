using Content.Shared.Administration.Notes;
using Content.Shared.Database;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.PlaytimeShare;

// TODO: Clean this up, make it as small as possbile (no = "" etc...)
[DataDefinition, Serializable]
public sealed partial class PlaytimeShareFormat
{
    [DataField]
    public string PublicKey = "";

    [DataField]
    public string PlayerName = "";

    [DataField]
    public NetUserId PlayerUid;

    [DataField]
    public DateTime Date;

    // Tracker name -> minutes
    // TODO : see what happens when no inputs and stuff
    // TODO: Also write tests for this stuff...
    [DataField]
    public Dictionary<string, TimeSpan> PlaytimeTrackers = new();

    [DataField]
    public List<SharedAdminNote> Notes = new();

    [DataField]
    public bool WhitelistStatus;
}

[DataDefinition, Serializable]
public sealed partial class PlaytimeShareFormatSigned
{
    [DataField]
    public PlaytimeShareFormat Format = new();

    [DataField]
    public string Signature = "";

    [DataField]
    public string Version = "1";
}

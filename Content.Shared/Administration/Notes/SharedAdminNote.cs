using Content.Shared.Database;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class SharedAdminNote
{
    [DataField]
    public int Id;

    [DataField]
    public NetUserId Player;

    [DataField]
    public int? Round;

    [DataField]
    public string? ServerName;

    [DataField]
    public TimeSpan PlaytimeAtNote;

    [DataField]
    public NoteType NoteType;

    [DataField]
    public string Message;

    [DataField]
    public NoteSeverity? NoteSeverity;

    [DataField]
    public bool Secret;

    [DataField]
    public string CreatedByName;

    [DataField]
    public string EditedByName;

    [DataField]
    public DateTime CreatedAt;

    [DataField]
    public DateTime? LastEditedAt;

    [DataField]
    public DateTime? ExpiryTime;

    [DataField]
    public string[]? BannedRoles;

    [DataField]
    public DateTime? UnbannedTime;

    [DataField]
    public string? UnbannedByName;

    [DataField]
    public bool? Seen;
}

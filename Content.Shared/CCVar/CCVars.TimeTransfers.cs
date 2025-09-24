using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<string> PlaytimePrivateKey =
        CVarDef.Create("timetransfers.privatekey", "", CVar.SERVERONLY |  CVar.CONFIDENTIAL);

    public static readonly CVarDef<bool> TimeTransferEnabled =
        CVarDef.Create("timetransfers.time_transfer_enabled", false, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<bool> AllowNewIncomingTransfers =
        CVarDef.Create("timetransfers.allow_new_incoming_transfers", true, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<bool> AllowNewOutgoingTransfers =
        CVarDef.Create("timetransfers.allow_new_outgoing_transfers", true, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<bool> AllowUnverifiedTransfers =
        CVarDef.Create("timetransfers.allow_unverified_transfers", false, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<int> TransferMaxNoteLength =
        CVarDef.Create("timetransfers.transfer_max_note_length", 999, CVar.SERVER | CVar.REPLICATED);
}

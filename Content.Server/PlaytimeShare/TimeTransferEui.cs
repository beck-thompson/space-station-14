using System.Threading.Tasks;
using Content.Server.Administration.Notes;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Server.Players.PlayTimeTracking;
using Content.Shared.CCVar;
using Content.Shared.Eui;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.PlaytimeShare;
using Robust.Shared.Configuration;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.PlaytimeShare;

public sealed class TimeTransferEui : BaseEui
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly ISharedPlaytimeManager _playtime = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly PlaytimeShareManager _playtimeShare = default!;
    [Dependency] private readonly ISerializationManager _serialization = default!;
    [Dependency] private readonly PlayTimeTrackingManager _tracking = default!;

    public TimeTransferEui()
    {
        IoCManager.InjectDependencies(this);
    }

    public override async void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        switch (msg)
        {
            case GetPlaytimeRequest request:
            {
                SendMessage(new GotTimeTransfer(await GeneratePlaytimeShareFormat()));
                break;
            }
            case SetPlaytimeRequest request:
            {
                await TryAddPlaytime(request.Playtime, request.Note);
                break;
            }
            case GetPublicKeyRequest:
            {
                var key = _playtimeShare.GetPublicKey();
                SendMessage(new GotPublicKey(key));
                break;
            }
        }
    }

    private async Task TryAddPlaytime(PlaytimeShareFormatSigned signedPlaytime, string note)
    {
        // TODO: first ensure the public key is in the DB of course xD
        var valid = _playtimeShare.VerifyString(signedPlaytime.Format.PublicKey, signedPlaytime.Signature, _serialization.WriteValue(signedPlaytime.Format, alwaysWrite: true, notNullableOverride: true).ToString());

        if (!valid)
            return;

        _playtimeShare.AddTimeTransfer(signedPlaytime, note);
        _tracking.QueueRefreshTrackers(Player);
    }

    private async Task<string> GeneratePlaytimeShareFormat()
    {
        var playtimeShare = new PlaytimeShareFormat
        {
            PublicKey = _playtimeShare.GetPublicKey(),
            PlayerName = Player.Name,
            PlayerUid = Player.UserId,
            Date = DateTime.UtcNow,
            PlaytimeTrackers = new Dictionary<string, TimeSpan>(_playtime.GetPlayTimes(Player)),
        };

        var visibleNotesServer = await _db.GetVisibleAdminNotes(Player.UserId);

        foreach (var note in visibleNotesServer)
        {
            playtimeShare.Notes.Add(note.ToShared());
        }

        playtimeShare.WhitelistStatus = await _db.GetWhitelistStatusAsync(Player.UserId);
        var signed = new PlaytimeShareFormatSigned
        {
            Format = playtimeShare,
            Signature = _playtimeShare.SignString(_serialization.WriteValue(playtimeShare, alwaysWrite: true, notNullableOverride: true).ToString()),
        };

        var dataNode = _serialization.WriteValue(signed, alwaysWrite: true, notNullableOverride: true);

        return dataNode.ToString();
    }
}

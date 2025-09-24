using Content.Server.Database;
using Content.Shared.Administration.Notes;
using Content.Shared.Database;

namespace Content.Server.Administration.Notes;

public static class AdminNotesExtensions
{
    public static SharedAdminNote ToShared(this IAdminRemarksRecord note)
    {
        NoteSeverity? severity = null;
        var secret = false;
        NoteType type;
        string[]? bannedRoles = null;
        string? unbannedByName = null;
        DateTime? unbannedTime = null;
        bool? seen = null;
        switch (note)
        {
            case AdminNoteRecord adminNote:
                type = NoteType.Note;
                severity = adminNote.Severity;
                secret = adminNote.Secret;
                break;
            case AdminWatchlistRecord:
                type = NoteType.Watchlist;
                secret = true;
                break;
            case AdminMessageRecord adminMessage:
                type = NoteType.Message;
                seen = adminMessage.Seen;
                break;
            case ServerBanNoteRecord ban:
                type = NoteType.ServerBan;
                severity = ban.Severity;
                unbannedTime = ban.UnbanTime;
                unbannedByName = ban.UnbanningAdmin?.LastSeenUserName ?? Loc.GetString("system-user");
                break;
            case ServerRoleBanNoteRecord roleBan:
                type = NoteType.RoleBan;
                severity = roleBan.Severity;
                bannedRoles = roleBan.Roles;
                unbannedTime = roleBan.UnbanTime;
                unbannedByName = roleBan.UnbanningAdmin?.LastSeenUserName ?? Loc.GetString("system-user");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), note.GetType(), "Unknown note type");
        }

        // There may be bans without a user, but why would we ever be converting them to shared notes?
        if (note.Player is null)
            throw new ArgumentNullException(nameof(note), "Player user ID cannot be null for a note");

        return new SharedAdminNote
        {
            Id = note.Id,
            Player = note.Player!.UserId,
            Round = note.Round?.Id,
            ServerName = note.Round?.Server.Name,
            PlaytimeAtNote = note.PlaytimeAtNote,
            NoteType = type,
            Message = note.Message,
            NoteSeverity = severity,
            Secret = secret,
            CreatedByName = note.CreatedBy?.LastSeenUserName ?? Loc.GetString("system-user"),
            EditedByName = note.LastEditedBy?.LastSeenUserName ?? string.Empty,
            CreatedAt = note.CreatedAt.UtcDateTime,
            LastEditedAt = note.LastEditedAt?.UtcDateTime,
            ExpiryTime = note.ExpirationTime?.UtcDateTime,
            BannedRoles = bannedRoles,
            UnbannedTime = unbannedTime,
            UnbannedByName = unbannedByName,
            Seen = seen,
        };
    }
}

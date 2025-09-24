using Content.Server.Administration;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.PlaytimeShare;

[AdminCommand(AdminFlags.Admin)]
public sealed class TimeTransferAdminControl : LocalizedCommands
{
    [Dependency] private readonly EuiManager _eui = default!;

    public override string Command => "timetransferadmin";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player == null)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        var ui = new TimeTransferAdminEui();
        _eui.OpenEui(ui, shell.Player);
    }
}

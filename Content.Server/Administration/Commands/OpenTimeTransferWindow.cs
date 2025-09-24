using Content.Server.Administration.Logs;
using Content.Server.EUI;
using Content.Server.PlaytimeShare;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AnyCommand]
public sealed class OpenTimeTransferWindow : LocalizedEntityCommands
{
    [Dependency] private readonly EuiManager _euiManager = default!;

    public override string Command => "playtimetransferwindow";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        var ui = new TimeTransferEui();
        _euiManager.OpenEui(ui, player);
    }
}

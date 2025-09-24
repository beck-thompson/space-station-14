using System.Security.Cryptography;
using Content.Server.Administration;
using Content.Server.Players.PlayTimeTracking;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Content.Shared.CCVar;

namespace Content.Server.PlaytimeShare;

[AdminCommand(AdminFlags.Admin)]
public sealed class PlaytimeShareCommand : LocalizedCommands
{
    public override string Command => "playtimesharekeygen";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var cfg = IoCManager.Resolve<IConfigurationManager>();

        var rsa = RSA.Create(4096);

        cfg.SetCVar(CCVars.PlaytimePrivateKey, rsa.ExportRSAPrivateKeyPem());
        cfg.SaveToFile();
    }
}

using Content.Shared.PlaytimeShare;
using Robust.Client.Console;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Network;

namespace Content.Client.PlaytimeShare;

public sealed class TimeTransferButton : Button
{
    [Dependency] private readonly IClientNetManager _net = default!;
    [Dependency] private readonly IClientConsoleHost _console = default!;

    public TimeTransferButton()
    {
        IoCManager.InjectDependencies(this);

        Text = "Time transfer";
        OnPressed += OnOnPressed;
    }

    private void OnOnPressed(ButtonEventArgs obj)
    {
        _console.ExecuteCommand("playtimetransferwindow");
    }
}


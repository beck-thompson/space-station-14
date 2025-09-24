using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.PlaytimeShare;

namespace Content.Client.PlaytimeShare.TimeTransferAdmin;

// This uses a cascading type method. When the state comes in, it gets relayed to each part which in turn will relay
// it to any controls and stuff that needs it.
public sealed class TimeTransferAdminEui : BaseEui
{
    private TimeTransferAdminWindow _window;

    private readonly TimeTransferStatusControl _statusControl;
    private readonly TimeTransferServerViewControl _serverViewControl;
    private readonly TimeTransfersTransfersControl _transfersControl;

    public TimeTransferAdminEui()
    {
        IoCManager.InjectDependencies(this);

        _window = new TimeTransferAdminWindow();


        _statusControl = new TimeTransferStatusControl();
        _serverViewControl = new TimeTransferServerViewControl();
        _transfersControl = new TimeTransfersTransfersControl();

        _window.MasterTabContainer.AddChild(_statusControl);
        _window.MasterTabContainer.AddChild(_serverViewControl);
        _window.MasterTabContainer.AddChild(_transfersControl);

        _window.MasterTabContainer.SetTabTitle(0, "Status");
        _window.MasterTabContainer.SetTabTitle(1, "Trusted servers");
        _window.MasterTabContainer.SetTabTitle(2, "Transfers");

        _window.OpenCentered();

        // TODO: I have no clue what I meant below...
        // remember to close eui like admin log does
    }

    public override void HandleState(EuiStateBase incomingState)
    {
        if (incomingState is not TimeTransferAdminState state)
            return;

        _statusControl.HandleState(state);
        _serverViewControl.HandleState(state);
        _transfersControl.HandleState(state);
    }
}

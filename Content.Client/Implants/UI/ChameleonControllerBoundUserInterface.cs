﻿using Content.Shared.Clothing;
using Content.Shared.Implants;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Timing;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client.Implants.UI;

[UsedImplicitly]
public sealed class ChameleonControllerBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private readonly UseDelaySystem _delay;

    [ViewVariables]
    private ChameleonControllerMenu? _menu;

    public ChameleonControllerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _delay =  EntMan.System<UseDelaySystem>();
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<ChameleonControllerMenu>();
        _menu.OnJobSelected += OnJobSelected;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not ChameleonControllerBuiState)
            return;

        var jobProtos = _prototypeManager.EnumeratePrototypes<JobPrototype>();
        var validList = new List<JobPrototype>();

        // Only add stuff that actually has clothing! We don't want stuff like AI or borgs.
        foreach (var job in jobProtos)
        {
            if (job.StartingGear == null || !_prototypeManager.HasIndex<RoleLoadoutPrototype>(LoadoutSystem.GetJobPrototype(job.ID)))
                continue;

            validList.Add(job);
        }

        _menu?.UpdateState(validList);
    }

    private void OnJobSelected(ProtoId<JobPrototype> selectedJob)
    {
        if (!EntMan.TryGetComponent<UseDelayComponent>(Owner, out var useDelayComp))
            return;

        if (!_delay.TryResetDelay((Owner, useDelayComp), true))
            return;

        SendMessage(new ChameleonControllerSelectedJobMessage(selectedJob));

        if (!_delay.TryGetDelayInfo((Owner, useDelayComp), out var delay) || _menu == null)
            return;

        _menu._lockedUntil = DateTime.Now.Add(delay.Length);
        _menu.UpdateGrid(true);
    }
}

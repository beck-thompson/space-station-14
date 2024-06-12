using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.ReagentOnItem;
using Content.Shared.Throwing;
using Robust.Shared.Containers;
using Robust.Shared.Random;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Examine;
using Robust.Shared.GameStates;

namespace Content.Shared.ReagentOnItem;

public sealed class SpaceLubeOnItemSystem : EntitySystem
{
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpaceLubeOnItemComponent, ContainerGettingInsertedAttemptEvent>(OnHandPickUp);
        SubscribeLocalEvent<SpaceLubeOnItemComponent, ExaminedEvent>(OnExamine);

        SubscribeLocalEvent<SpaceLubeOnItemComponent, ComponentGetState>(GetSpaceLubeState);
        SubscribeLocalEvent<SpaceLubeOnItemComponent, ComponentHandleState>(HandleSpaceLubeState);
    }

    private void OnHandPickUp(EntityUid uid, SpaceLubeOnItemComponent component, ContainerGettingInsertedAttemptEvent args)
    {
        _inventory.TryGetSlotEntity(args.Container.Owner, "gloves", out var gloves);

        if (HasComp<NonStickSurfaceComponent>(gloves))
            return;

        if (component.EffectStacks < 1)
        {
            RemComp<SpaceLubeOnItemComponent>(uid);
            return;
        }

        var randDouble = _random.NextDouble();
        if (randDouble > 1 - component.ChanceToDecreaseReagentOnGrab)
        {
            component.EffectStacks -= 1;
        }

        Dirty(uid, component);

        args.Cancel();

        var user = args.Container.Owner;
        _transform.SetCoordinates(uid, Transform(user).Coordinates);
        _transform.AttachToGridOrMap(uid);
        _throwing.TryThrow(uid, _random.NextVector2(), strength: component.PowerOfThrowOnPickup);
        _popup.PopupPredicted(Loc.GetString("space-lube-on-item-slip", ("target", Identity.Entity(uid, EntityManager))), user, user, PopupType.MediumCaution);

    }

    private void OnExamine(EntityUid uid, SpaceLubeOnItemComponent comp, ExaminedEvent args)
    {
        if (args.IsInDetailsRange)
        {
            args.PushMarkup(Loc.GetString("space-lube-on-item-inspect"));
        }
    }

    private void HandleSpaceLubeState(EntityUid uid, SpaceLubeOnItemComponent component, ref ComponentHandleState args)
    {
        if (args.Current is not ReagentOnItemComponentState state)
            return;

        component.EffectStacks = state.EffectStacks;
        component.MaxStacks = state.MaxStacks;
    }

    private void GetSpaceLubeState(EntityUid uid, SpaceLubeOnItemComponent component, ref ComponentGetState args)
    {
        args.State = new ReagentOnItemComponentState(component.EffectStacks, component.MaxStacks);
    }
}

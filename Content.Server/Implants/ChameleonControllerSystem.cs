﻿using Content.Server.Clothing.Systems;
using Content.Server.Preferences.Managers;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Station;
using Content.Shared.Timing;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Implants;

public sealed class ChameleonControllerSystem : SharedChameleonControllerSystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly SharedStationSpawningSystem _stationSpawningSystem = default!;
    [Dependency] private readonly ChameleonClothingSystem _chameleonClothingSystem = default!;
    [Dependency] private readonly IServerPreferencesManager _preferences = default!;
    [Dependency] private readonly UseDelaySystem _delay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SubdermalImplantComponent, ChameleonControllerSelectedJobMessage>(OnSelected);
    }

    private void OnSelected(Entity<SubdermalImplantComponent> ent, ref ChameleonControllerSelectedJobMessage args)
    {
        if (!_delay.TryResetDelay(ent.Owner, true) || ent.Comp.ImplantedEntity == null || !HasComp<ChameleonControllerImplantComponent>(ent))
            return;

        ChangeChameleonClothingToJob(ent.Comp.ImplantedEntity.Value, args.SelectedJob);
    }

    /// <summary>
    ///     Switches all the chameleon clothing that the implant user is wearing to look like the selected job.
    /// </summary>
    private void ChangeChameleonClothingToJob(EntityUid user, ProtoId<JobPrototype> job)
    {
        if (!IsValidJob(job, out var jobPrototype))
            return;

        if (!_proto.TryIndex(jobPrototype.StartingGear, out var startingGearPrototype))
            return;

        if (!TryComp<ActorComponent>(user, out var actorComponent))
            return;

        var session = actorComponent.PlayerSession;
        var userId = actorComponent.PlayerSession.UserId;
        var prefs = _preferences.GetPreferences(userId);

        if (prefs.SelectedCharacter is not HumanoidCharacterProfile profile)
            return;

        var jobProtoId = LoadoutSystem.GetJobPrototype(job.Id);

        profile.Loadouts.TryGetValue(jobProtoId, out var loadout);
        loadout ??= new RoleLoadout(jobProtoId);
        loadout.SetDefault(profile, session, _proto); // only sets the default if the player has no loadout

        if (!_proto.HasIndex(loadout.Role))
            return;

        if (!_inventorySystem.TryGetSlots(user, out var slots))
            return;

        // Go through all the slots on the player
        foreach (var slot in slots)
        {
            _inventorySystem.TryGetSlotEntity(user, slot.Name, out var containedUid);
            // If there isn't anything there, or it isn't chameleon clothing.
            if (containedUid == null || !TryComp<ChameleonClothingComponent>(containedUid, out var chameleonClothingComponent))
                continue;

            // Either get the gear from the loadout, or the starting gear.
            var proto = _stationSpawningSystem.GetGearForSlot(loadout, slot.Name) ?? ((IEquipmentLoadout) startingGearPrototype).GetGear(slot.Name);
            if (proto == string.Empty)
                continue;

            _chameleonClothingSystem.SetSelectedPrototype(containedUid.Value, proto, true, chameleonClothingComponent);
        }
    }
}

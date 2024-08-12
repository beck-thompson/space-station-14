using Content.Shared.Chat;
using Content.Shared.Administration.Logs;
using Content.Shared.Popups;
using Content.Shared.Clothing;
using Content.Shared.Database;
using Content.Shared.Preferences;
using Content.Shared.Speech;
using Content.Shared.VoiceMask;
using Robust.Shared.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Actions;

namespace Content.Server.VoiceMask;

public sealed partial class VoiceMaskSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    private const string MaskSlot = "mask";
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VoiceMaskComponent, InventoryRelayedEvent<TransformSpeakerNameEvent>>(OnTransformSpeakerName);
        SubscribeLocalEvent<VoiceMaskComponent, VoiceMaskChangeNameMessage>(OnChangeName);
        SubscribeLocalEvent<VoiceMaskComponent, VoiceMaskChangeVerbMessage>(OnChangeVerb);
        SubscribeLocalEvent<VoiceMaskComponent, ClothingGotEquippedEvent>(OnEquip);
        SubscribeLocalEvent<VoiceMaskSetNameEvent>(OpenUI);
    }

    private void OnTransformSpeakerName(Entity<VoiceMaskComponent> entity, ref InventoryRelayedEvent<TransformSpeakerNameEvent> args)
    {
        args.Args.VoiceName = entity.Comp.VoiceMaskName;
        args.Args.SpeechVerb = entity.Comp.VoiceMaskSpeechVerb ?? args.Args.SpeechVerb;
    }

    #region User inputs from UI
    private void OnChangeVerb(Entity<VoiceMaskComponent> ent, ref VoiceMaskChangeVerbMessage msg)
    {
        if (msg.Verb is { } id && !_proto.HasIndex<SpeechVerbPrototype>(id))
            return;

        ent.Comp.VoiceMaskSpeechVerb = msg.Verb;
        // verb is only important to metagamers so no need to log as opposed to name

        _popupSystem.PopupEntity(Loc.GetString("voice-mask-popup-success"), ent, msg.Actor);

        UpdateUI(ent, ent.Comp);
    }

    private void OnChangeName(EntityUid uid, VoiceMaskComponent component, VoiceMaskChangeNameMessage message)
    {
        if (message.Name.Length > HumanoidCharacterProfile.MaxNameLength || message.Name.Length <= 0)
        {
            _popupSystem.PopupEntity(Loc.GetString("voice-mask-popup-failure"), uid, message.Actor, PopupType.SmallCaution);
            return;
        }

        component.VoiceMaskName = message.Name;
        _adminLogger.Add(LogType.Action, LogImpact.Medium, $"{ToPrettyString(message.Actor):player} set voice of {ToPrettyString(uid):mask}: {component.VoiceMaskName}");

        _popupSystem.PopupEntity(Loc.GetString("voice-mask-popup-success"), uid, message.Actor);

        UpdateUI(uid, component);
    }
    #endregion

    #region UI
    private void OnEquip(EntityUid uid, VoiceMaskComponent component, ClothingGotEquippedEvent args)
    {
        _actions.AddAction(args.Wearer, ref component.ActionEntity, component.Action, uid);
    }

    private void OpenUI(VoiceMaskSetNameEvent ev)
    {
        if (!_inventory.TryGetSlotEntity(ev.Performer, MaskSlot, out var maskEntity))
            return;

        if (!_uiSystem.HasUi(maskEntity.Value, VoiceMaskUIKey.Key))
            return;

        _uiSystem.OpenUi(maskEntity.Value, VoiceMaskUIKey.Key, ev.Performer);
        UpdateUI(maskEntity.Value);
    }

    private void UpdateUI(EntityUid owner, VoiceMaskComponent? component = null)
    {
        if (!Resolve(owner, ref component))
            return;

        if (_uiSystem.HasUi(owner, VoiceMaskUIKey.Key))
            _uiSystem.SetUiState(owner, VoiceMaskUIKey.Key, new VoiceMaskBuiState(component.VoiceMaskName, component.VoiceMaskSpeechVerb));
    }
    #endregion
}

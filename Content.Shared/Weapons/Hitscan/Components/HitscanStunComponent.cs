using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Hitscan.Components;

/// <summary>
/// Hitscan entities that have this component will stun the target.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HitscanStunComponent : Component
{
    [DataField]
    public float StaminaDamage = 10.0f;
}

using System.Numerics;
using Content.Shared.BulletHole;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Client.BulletHole;

public sealed class BulletHoleSystem : VisualizerSystem<BulletHoleVisualsComponent>
{
    [Dependency] private readonly IRobustRandom _rand = default!;

    protected override void OnAppearanceChange(EntityUid uid, BulletHoleVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (!AppearanceSystem.TryGetData<(Vector2, Vector2)>(uid, BulletHoleVisuals.BulletHole, out var bulletHole))
            return;

        var bulletholelocation = bulletHole.Item1;
        var velocity = bulletHole.Item2.Normalized();
        // var bulletVelocity = bulletHole.Item3;

        // SpriteSystem.AddRsiLayer(uid, new RSI.StateId("bullethole"), new ResPath("Clothing/Belt/assault.rsi"));
        var layer = SpriteSystem.AddLayer(uid, new SpriteSpecifier.Rsi(new ResPath("Structures/Walls/solid.rsi/bullethole.png"), "bullethole"));
        SpriteSystem.LayerMapSet(uid, "bulletmap", layer);
        SpriteSystem.LayerSetRsiState(uid, "bulletmap", "bullethole");
        SpriteSystem.LayerSetRotation(uid, "bulletmap", _rand.NextAngle());
        SpriteSystem.LayerSetOffset(uid, "bulletmap", bulletholelocation + 0.20f * velocity);
    }
}

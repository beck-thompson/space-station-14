using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.PlaytimeShare;


[Serializable, NetSerializable]
public sealed class GetPlaytimeRequest : EuiMessageBase;

[Serializable, NetSerializable]
public sealed class GetPublicKeyRequest : EuiMessageBase;

[Serializable, NetSerializable]
public sealed class SetPlaytimeRequest : EuiMessageBase
{
    public PlaytimeShareFormatSigned Playtime = new();

    public string Note = "";
};

[Serializable, NetSerializable]
public sealed class GotPublicKey(string publicKey) : EuiMessageBase
{
    public string PublicKey = publicKey;
}

[Serializable, NetSerializable]
public sealed class GotTimeTransfer(string publicKey) : EuiMessageBase
{
    public string TimeTransfer = publicKey;
}

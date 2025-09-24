using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.PlaytimeShare;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Server.PlaytimeShare;

public sealed class PlaytimeShareManager
{
    [Dependency] private readonly IServerNetManager _netManager = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ISerializationManager _serialization = default!;

    private IConfigurationManager _cfg = default!;

    public string GetPublicKey()
    {
        var privateKey = _cfg.GetCVar(CCVars.PlaytimePrivateKey);

        var rsa = new RSACryptoServiceProvider(4096);
        rsa.ImportFromPem(privateKey);

        return rsa.ExportRSAPublicKeyPem();
    }

    public PrivateKeyStatus GetPrivateKeyStatus()
    {
        var privateKey = _cfg.GetCVar(CCVars.PlaytimePrivateKey);

        if (privateKey == "")
            return PrivateKeyStatus.Empty;

        var rsa = new RSACryptoServiceProvider(4096);
        try
        {
            rsa.ImportFromPem(privateKey);
        }
        catch (Exception e)
        {
            return PrivateKeyStatus.Invalid;
        }

        return PrivateKeyStatus.Valid;
    }

    public string SignString(string input)
    {
        var privateKey = _cfg.GetCVar(CCVars.PlaytimePrivateKey);

        var rsa = new RSACryptoServiceProvider(4096);
        rsa.ImportFromPem(privateKey);

        // TODO: Should I be using UTF8 or Unicode??? figure this out...
        var bytesPlainTextData = Encoding.UTF8.GetBytes(input);
        // For some reason you can't use sha3 don't ask why
        var signedByeArrData = rsa.SignData(bytesPlainTextData, SHA384.Create());

        return Convert.ToBase64String(signedByeArrData);
    }

    // Pub key is in .pem format.
    public bool VerifyString(string publicKey, string signature, string data)
    {
        var RSA = new RSACryptoServiceProvider(4096);

        // Yes for some reason the import from pem has overloads so it's the same for public and private...
        // This feels crazy to me - when would you ever want it to DYNAMICALLY choose?? Seems like a recipe for disaster
        RSA.ImportFromPem(publicKey);

        var signatureBytes = Convert.FromBase64String(signature);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        var valid = RSA.VerifyData(dataBytes, SHA384.Create(), signatureBytes);

        return valid;
    }

    public async Task<List<TimeTransferServerInfoSerializable>> GetAllTrustedServerInfo()
    {
        var serverInfo = await _db.GetAllTimeTransferServerInfo();
        List<TimeTransferServerInfoSerializable> servers = new();

        foreach(var server in serverInfo)
        {
            // var roleTransferData = server.RoleTransferData.Deserialize<Dictionary<string, string>>();
            // if (roleTransferData == null)
            //     continue; // TODO: Throw error here...

            servers.Add(new TimeTransferServerInfoSerializable
            {
                ServerPublicKey = server.ServerPublicKey,
                ServerName = server.ServerName,
                Enabled = server.Enabled,
                AutoApproveTransfers = server.AutoApproveTransfers,
                ApplicationMaxAge = server.ApplicationMaxAge,
                RoleTransferData = new Dictionary<string, string>(),
                Note = server.Note,
            });
        }

        return servers;
    }

    public void AddTimeTransfer(PlaytimeShareFormatSigned signed, string note = "")
    {
        var raw = _serialization.WriteValue(signed.Format, alwaysWrite: true, notNullableOverride: true).ToString();

        _db.AddTimeTransfer(signed, raw, note);
    }

    // todo make new function that only gets approved transfers
    public async Task<List<PlaytimeShareFormatSigned>> GetTimeTransfers(NetUserId player)
    {
        var transfersDb = await _db.GetTimeTransfers(player);

        List<PlaytimeShareFormatSigned> formatedList = new();

        foreach(var transfer in transfersDb)
        {
            var formated = new PlaytimeShareFormatSigned();
            formated.Signature = transfer.Signature;
            formated.Version = transfer.Version;


            // TODO: Fix - do not ask why this is necessary
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(transfer.Raw));
            using var reader = new StreamReader(stream, EncodingHelpers.UTF8);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);

            var root = yamlStream.Documents[0].RootNode;

            var playtime = _serialization.Read<PlaytimeShareFormat>(root.ToDataNode(), notNullableOverride: true);

            formated.Format = playtime;
            formatedList.Add(formated);
        }

        return formatedList;
    }

    public void Initialize()
    {
        _cfg = IoCManager.Resolve<IConfigurationManager>();
    }
}

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.PlaytimeShare;
using Robust.Client.UserInterface;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Client.PlaytimeShare;

public sealed class TimeTransferEui : BaseEui
{
    [Dependency] private readonly IClipboardManager _clipboard = default!;
    [Dependency]  private readonly IFileDialogManager _dialogManager = default!;
    [Dependency]  private readonly ISerializationManager _serialization = default!;

    private TimeTransferWindow _window;

    public TimeTransferEui()
    {
        IoCManager.InjectDependencies(this);

        _window = new TimeTransferWindow();
        _window.OpenCentered();

        _window.GetPlaytime.OnPressed += _ => GetPlaytimePressed();
        _window.SubmitPlaytime.OnPressed += _ => SubmitPlaytime();
        _window.GetPublicKey.OnPressed += _ => GetGetPublicKey();

        _window.CopyKey.OnPressed += _ =>
        {
            _clipboard.SetText(_window.Output.Text ?? "");
        };

        // remember to close eui like admin log does
    }

    private void GetPlaytimePressed()
    {
        SendMessage(new GetPlaytimeRequest());
    }

    private async Task SubmitPlaytime()
    {
        await using var file = await _dialogManager.OpenFile();

        if (file == null)
            return;

        using var reader = new StreamReader(file, EncodingHelpers.UTF8);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);

        var root = yamlStream.Documents[0].RootNode;

        var playtime = _serialization.Read<PlaytimeShareFormatSigned>(root.ToDataNode(), notNullableOverride: true);

        SendMessage(new SetPlaytimeRequest { Playtime = playtime, Note = "this is a test" });
    }

    private void GetGetPublicKey()
    {
        SendMessage(new GetPublicKeyRequest());
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        switch (msg)
        {
            case GotPublicKey gotPublicKey:
            {
                _window.Output.Text = gotPublicKey.PublicKey;
                break;
            }

            case GotTimeTransfer gotTimeTransfer:
            {

                // _window.Output.Text = gotTimeTransfer.TimeTransfer;
                SaveFile(gotTimeTransfer.TimeTransfer);

                break;
            }

        }
    }

    private async void SaveFile(string fileContents)
    {
        var file = await _dialogManager.SaveFile();

        if (file == null)
            return;

        try
        {
            await using var writer = new StreamWriter(file.Value.fileStream, Encoding.UTF8);
            await writer.WriteAsync(fileContents);
        }
        catch (Exception exc)
        {
            // _sawmill.Error($"Error when exporting profile\n{exc.StackTrace}");
        }
        finally
        {
            // EndExport();
            await file.Value.fileStream.DisposeAsync();
        }

        return;
    }
}

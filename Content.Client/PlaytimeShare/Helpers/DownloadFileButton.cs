using System.IO;
using System.Text;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.PlaytimeShare.Helpers;

public sealed class DownloadFileButton : Button
{
    [Dependency] private readonly IFileDialogManager _file = default!;
    private readonly ISawmill _sawmill;

    private string _downloadText;
    private bool _beingUsed;

    [ViewVariables]
    public string DownloadText { get => _downloadText; set => DownloadUpdated(value); }

    public DownloadFileButton()
    {
        IoCManager.InjectDependencies(this);

        _sawmill = Logger.GetSawmill("DownloadFileButton");

        _downloadText = string.Empty;

        OnPressed += OnDownloadButtonPressed;
    }

    private void DownloadUpdated(string downloadText)
    {
        if (_beingUsed)
            return;

        Disabled = downloadText == string.Empty;
        _downloadText = downloadText;
    }

    private async void OnDownloadButtonPressed(ButtonEventArgs args)
    {
        _beingUsed = true;

        var file = await _file.SaveFile();

        // Canceled
        if (file == null)
        {
            _beingUsed = false;
            return;
        }

        try
        {
            await using var writer = new StreamWriter(file.Value.fileStream, Encoding.UTF8);
            await writer.WriteAsync(_downloadText);
        }
        catch (Exception exc)
        {
            _sawmill.Error($"Error when downloading file \n{exc.StackTrace}");
        }
        finally
        {
            await file.Value.fileStream.DisposeAsync();
        }

        _beingUsed = false;
    }
}

namespace SFTPManager.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Notification.Wpf;
    using SFTPManager.Models;
    using SFTPManager.Services;

    public class TerminalViewModel : ObservableObject, IDisposable
    {
        private SftpSettings settings;
        private string terminalOutput;
        private string command;
        private readonly SftpService sftpService;
        private readonly NotificationService notificationService;

        public TerminalViewModel()
        {
            SendCommand = new AsyncRelayCommand(SendCommandToServerAsync);
            settings = new SftpSettings();
            sftpService = SftpService.Instance;
            notificationService = new NotificationService();
        }

        public SftpSettings Settings
        {
            get => settings;
            set => SetProperty(ref settings, value);
        }

        public string TerminalOutput
        {
            get => terminalOutput;
            set => SetProperty(ref terminalOutput, value);
        }

        public string Command
        {
            get => command;
            set => SetProperty(ref command, value);
        }

        public IAsyncRelayCommand SendCommand { get; }

        private async Task SendCommandToServerAsync()
        {
            if (string.IsNullOrEmpty(Command))
            {
                notificationService.ShowNotification("Command", "Please enter a command.", NotificationType.Warning);
                return;
            }

            var sftpCommand = sftpService.CreateCommand(Command);
            if (sftpCommand == null)
            {
                TerminalOutput += "\nError: Not connected to SFTP server";
                return;
            }

            string originalCommand = Command;
            Command = string.Empty;

            try
            {
                var result = await Task.Run(() =>
                {
                    try
                    {
                        var commandResult = sftpCommand.Execute();
                        if (sftpCommand.ExitStatus != 0)
                        {
                            throw new InvalidOperationException($"Command failed with status {sftpCommand.ExitStatus}: {sftpCommand.Error}");
                        }

                        return commandResult;
                    }
                    catch (Exception ex)
                    {
                        TerminalOutput += $"\nCommand execution error: {ex.Message}";
                        throw;
                    }
                });

                if (string.IsNullOrEmpty(result))
                {
                    throw new InvalidOperationException("No output generated after executing the command.");
                }

                TerminalOutput += $"\n$ {originalCommand}\n{result}";
            }
            catch (Exception ex)
            {
                TerminalOutput += $"\nError: {ex.Message}";
                notificationService.ShowNotification("Command Error", $"Failed to execute command: {ex.Message}", NotificationType.Error);
            }
        }

        public void Dispose()
        {
            sftpService?.Dispose();
        }
    }
}

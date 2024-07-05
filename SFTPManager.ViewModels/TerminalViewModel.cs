namespace SFTPManager.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Notification.Wpf;
    using Renci.SshNet.Common;
    using SFTPManager.Core;
    using SFTPManager.Core.Interfaces;
    using SFTPManager.Models;
    using SFTPManager.Resources;
    using SFTPManager.Services;

    public class TerminalViewModel : ObservableObject, IDisposable
    {
        private SftpSettings settings;
        private string terminalOutput;
        private string command;
        private readonly NotificationService notificationService;
        private readonly SshService sshService;
        private readonly ISftpSettingsProvider settingsProvider;

        public TerminalViewModel()
        {
            SendCommand = new AsyncRelayCommand(SendCommandToServerAsync);
            notificationService = new NotificationService();
            sshService = new SshService();
            settingsProvider = SftpSettingsProvider.Instance;
            LoadSettings();
        }

        public SftpSettings Settings
        {
            get => settings;
            set
            {
                SetProperty(ref settings, value);
                settingsProvider.SetSettings(value);
            }
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

        private void LoadSettings()
        {
            Settings = settingsProvider.GetSettings();
        }

        public async Task SendCommandToServerAsync()
        {
            if (string.IsNullOrEmpty(Command))
            {
                notificationService.ShowNotification(Resources_en.CommandTitle, Resources_en.CommandPrompt, NotificationType.Warning);
                return;
            }

            try
            {

                string originalCommand = Command;
                Command = string.Empty;

                await sshService.ConnectAsync(Settings);

                if (originalCommand.StartsWith("cd "))
                {
                    var cdCommand = originalCommand.Substring(3).Trim();
                    var result = await sshService.ExecuteCommandAsync($"cd {cdCommand}; pwd");

                    TerminalOutput += $"\n$ {originalCommand}\n{result}";
                }
                else
                {
                    var result = await sshService.ExecuteCommandAsync(originalCommand);

                    TerminalOutput += $"\n$ {originalCommand}\n{result}";
                }

                sshService.Disconnect();
            }
            catch (SshAuthenticationException ex)
            {
                TerminalOutput += $"\nError: {ex.Message}";
                notificationService.ShowNotification(Resources_en.CommandErrorTitle, string.Format(Resources_en.CommandErrorMessage, ex.Message), NotificationType.Error);
            }
            catch (Exception ex)
            {
                TerminalOutput += $"\nError: {ex.Message}";
                notificationService.ShowNotification(Resources_en.CommandErrorTitle, string.Format(Resources_en.CommandErrorMessage, ex.Message), NotificationType.Error);
            }
        }

        public void Dispose()
        {
            sshService?.Dispose();
        }
    }
}

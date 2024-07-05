namespace SFTPManager.Services
{
    using Notification.Wpf;
    using Renci.SshNet;
    using Renci.SshNet.Common;
    using SFTPManager.Models;
    using SFTPManager.Resources;
    using SFTPManager.Services;
    using System;
    using System.Threading.Tasks;
    public class SshService : IDisposable
    {
        private static readonly Lazy<SshService> instance = new Lazy<SshService>(() => new SshService());
        private readonly NotificationService notificationService;
        private SshClient sshClient;
        private ShellStream shellStream;
        private bool isConnected;

        private SshService()
        {
            notificationService = new NotificationService();
        }

        public static SshService Instance => instance.Value;

        public async Task<string> ConnectAsync(SftpSettings settings)
        {
            if (isConnected)
            {
                throw new InvalidOperationException("Already connected via SSH");
            }

            sshClient = new SshClient(settings.Server, settings.Port, settings.Username, settings.Password);

            try
            {
                await Task.Run(() => sshClient.Connect());

                shellStream = sshClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
                isConnected = sshClient.IsConnected;

                return isConnected ? Resources_en.ConnectionSuccessful : Resources_en.ConnectionUnsuccessful;
            }
            catch (SshAuthenticationException ex)
            {
                notificationService.ShowNotification("Authentication Error", $"Permission denied (password): {ex.Message}", NotificationType.Error);
                throw new InvalidOperationException("Authentication failed: Permission denied (password).", ex);
            }
            catch (Exception ex)
            {
                notificationService.ShowNotification("Connection Error", $"Failed to connect: {ex.Message}", NotificationType.Error);
                throw new InvalidOperationException("Failed to connect.", ex);
            }
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                sshClient.Disconnect();
                sshClient.Dispose();
                isConnected = false;
            }
            else
            {
                throw new InvalidOperationException("Not connected via SSH");
            }
        }

        public async Task<string> ExecuteCommandAsync(string commandText)
        {
            EnsureConnected();

            if (shellStream == null)
            {
                throw new InvalidOperationException("Shell stream not available. SSH command execution is not supported.");
            }

            var result = await Task.Run(() =>
            {
                try
                {
                    shellStream.WriteLine(commandText);
                    System.Threading.Thread.Sleep(500);
                    string output = ReadStream(shellStream);
                    return output;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Command execution error: {ex.Message}");
                }
            });

            return result;
        }

        private string ReadStream(ShellStream stream)
        {
            var output = new System.Text.StringBuilder();
            string line;
            while ((line = stream.ReadLine(TimeSpan.FromMilliseconds(100))) != null)
            {
                output.AppendLine(line);
            }
            return output.ToString();
        }

        public void Dispose()
        {
            Disconnect();
        }

        private void EnsureConnected()
        {
            if (!isConnected)
            {
                throw new InvalidOperationException("Not connected via SSH");
            }
        }
    }
}

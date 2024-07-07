namespace SFTPManager.Resources
{
    using System.Globalization;

    public static class LocalizationManager
    {
        public static string DisconnectedSuccessfully => GetString(nameof(DisconnectedSuccessfully));
        public static string AccessDenied => GetString(nameof(AccessDenied));
        public static string CommandErrorMessage => GetString(nameof(CommandErrorMessage));
        public static string CommandErrorTitle => GetString(nameof(CommandErrorTitle));
        public static string CommandPrompt => GetString(nameof(CommandPrompt));
        public static string CommandTitle => GetString(nameof(CommandTitle));
        public static string ConnectionSuccessful => GetString(nameof(ConnectionSuccessful));
        public static string ConnectionUnsuccessful => GetString(nameof(ConnectionUnsuccessful));
        public static string DownloadSuccess => GetString(nameof(DownloadSuccess));
        public static string ErrorSelectPaths => GetString(nameof(ErrorSelectPaths));
        public static string ErrorTitle => GetString(nameof(ErrorTitle));
        public static string FileDownloadFailed => GetString(nameof(FileDownloadFailed));
        public static string FileExists => GetString(nameof(FileExists));
        public static string FileSystemLoaded => GetString(nameof(FileSystemLoaded));
        public static string FileUploadFailed => GetString(nameof(FileUploadFailed));
        public static string UploadSuccess => GetString(nameof(UploadSuccess));
        public static string ValidationInvalidHostOrIp => GetString(nameof(ValidationInvalidHostOrIp));
        public static string ValidationServerUsernameEmpty => GetString(nameof(ValidationServerUsernameEmpty));
        public static string Ukrainian => GetString(nameof(Ukrainian));
        public static string English => GetString(nameof(English));
        public static string Light => GetString(nameof(Light));
        public static string Dark => GetString(nameof(Dark));
        private static string GetString(string key)
        {
            return typeof(Resources_en).GetProperty(key).GetValue(null, null).ToString();
        }
    }
}
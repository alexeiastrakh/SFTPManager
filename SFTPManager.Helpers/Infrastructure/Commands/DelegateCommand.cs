namespace SFTPManager.Infrastructure.Commands
{
    using SFTPManager.Infrastructure.Commands.Base;

    public class DelegateCommand : BaseCommand
    {
        private readonly Action<object?>? _execute;
        private readonly Func<object?, bool>? _canExecute;

        public DelegateCommand(Action<object?>? execute, Func<object?, bool>? canExecute = null) 
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}

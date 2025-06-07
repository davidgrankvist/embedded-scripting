using System.Windows.Input;

namespace GraphicsSandbox.App.ViewModel.Framework
{
    class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action execute)
            : this(execute, () => true)
        {
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute();
        }

        public void Execute(object? parameter)
        {
            execute();
        }
    }
}

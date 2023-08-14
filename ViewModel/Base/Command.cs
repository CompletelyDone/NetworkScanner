using System.Windows.Input;

namespace ViewModel.Base
{
    public class Command : ICommand
    {
        public Command(Action action, Func<bool> canExecute = null!) 
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        private readonly Action action;
        private readonly Func<bool> canExecute;
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? param)
        {
            return canExecute == null ? true : canExecute();
        }

        public void Execute(object? param)
        {
            if(CanExecute(param))
            {
                action.Invoke();
            }
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

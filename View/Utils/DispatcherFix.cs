using System.Diagnostics;
using System.Threading;
using System;
using System.Windows.Threading;
using NetworkScanner.ViewModel.Interfaces;

namespace View.Utils
{
    public sealed class DispatcherFix : IDispatcherFix
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherFix(Dispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            this._dispatcher.Invoke(action);
        }
    }
}

using System.Windows;
using ViewModel.Interfaces;

namespace View.Utils
{
    public class ErrorGenerator : IErrorGenerator
    {
        public ErrorGenerator() { }

        public void GenerateError(string message) 
        { 
            if(string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) MessageBox.Show("Unknown error");
            else MessageBox.Show(message);
        }
    }
}

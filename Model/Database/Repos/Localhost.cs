using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace Model.Database.Repos
{
    public class Localhost : BaseEntity, INotifyPropertyChanged
    {
        private IPAddress localIP;
        public IPAddress LocalIP
        {
            get
            {
                return localIP;
            }
            set
            {
                localIP = value;
                OnPropertyChanged("LocalIP");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] String prop = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}

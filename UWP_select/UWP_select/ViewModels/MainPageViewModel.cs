using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UWP_select.Network;

namespace UWP_select.ViewModels
{
    class MainPageViewModel : BindableBase
    {
        public ICommand CmdBegin { get; private set; }
        public ICommand CmdStop { get; private set; }

        public MainPageViewModel()
        {
            CmdBegin = new DelegateCommand(OnBegin);
            CmdStop = new DelegateCommand(OnStop);

            sockadapter1.Begin(this);
            sockadapter2.Begin(this);
            sockadapter3.Begin(this);
        }

        private SocketAdapter sockadapter1 = new SocketAdapter(11001);
        private SocketAdapter sockadapter2 = new SocketAdapter(11002);
        private SocketAdapter sockadapter3 = new SocketAdapter(11003);

        private string _tbxMsg = "Test";
        public string TbxMsg
        {
            get { return _tbxMsg; }
            set { SetProperty(ref _tbxMsg, value); }
        }

        private void OnBegin()
        {
            sockadapter1.Begin(this);
            sockadapter2.Begin(this);
            sockadapter3.Begin(this);
        }

        private void OnStop()
        {
            sockadapter1.Stop();
            sockadapter2.Stop();
            sockadapter3.Stop();
        }
    }
}

using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using NetFwTypeLib;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using System.Linq;
using GraphicRdpScopeToggler.Services.RdpService;
using System.Collections.ObjectModel;
using static System.Windows.Forms.Design.AxImporter;
using System.Diagnostics;
using System.Data;
using GraphicRdpScopeToggler.Models;
using GraphicRdpScopeToggler.Services.FilesService;
using System.Collections.Generic;

namespace GraphicRdpScopeToggler.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties
        private string _title = "RDP Scope Toggler";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string countDownHour;
        public string CountDownHour
        {
            get { return countDownHour; }
            set { SetProperty(ref countDownHour, value); }
        }

        private string countDownMinute;
        public string CountDownMinute
        {
            get { return countDownMinute; }
            set { SetProperty(ref countDownMinute, value); }
        }

        public ObservableCollection<RunningTimerModel> ActiveTimers { get; } = new();


        private string selectedAction;
        public string SelectedAction
        {
            get => selectedAction;
            set => SetProperty(ref selectedAction, value); // Updates UI when value changes
        }
        public ObservableCollection<string> Options { get; }

        public ICommand StartCommand { get; set; }
        public ICommand OpenForAllCommand { get; set; }
        public ICommand CloseForAllCommand { get; set; }
        public ICommand OpenForWhiteListCommand { get; set; }
        public ICommand OpenForLocalComputersCommand { get; set; }
        public ICommand OpenForLocalComputersAndForWhiteListCommand { get; set; }

        #endregion

        private readonly IRdpService _rdpService;

        public MainWindowViewModel(IRdpService rdpService)
        {
            Options = new ObservableCollection<string>
            {
                "Open for white list",
                "Open for local computers",
                "Open for local computers and for white list",
                "Open for all",
                "Close for all",
            };
            SelectedAction = Options[0];

            _rdpService = rdpService;
            CountDownMinute = "01";
            CountDownHour = "00";
            StartCommand = new DelegateCommand(StartCountDownTask);
            OpenForAllCommand = new DelegateCommand(OpenForAll);
            CloseForAllCommand = new DelegateCommand(CloseForAll);
            OpenForWhiteListCommand = new DelegateCommand(OpenForWhiteList);
            OpenForLocalComputersCommand = new DelegateCommand(OpenForLocalComputers);
            OpenForLocalComputersAndForWhiteListCommand = new DelegateCommand(OpenForLocalComputersAndForWhiteList);
        }

        private void OpenForLocalComputersAndForWhiteList()
        {
            _rdpService.OpenRdpForLocalComputersAndForWhiteList();
        }

        private void OpenForWhiteList()
        {
            _rdpService.OpenRdpForWhiteList();
        }

        private void OpenForLocalComputers()
        {
            _rdpService.OpenRdpForLocalComputers();
        }

        private void CloseForAll()
        {
            _rdpService.CloseRdpForAll();
        }

        private void OpenForAll()
        {
            _rdpService.OpenRdpForAll();
        }

        private void StartCountDownTask()
        {
            int hours = int.Parse(CountDownHour);
            int minutes = int.Parse(CountDownMinute);
            var duration = new TimeSpan(hours, minutes, 0);

            switch (SelectedAction)
            {
                case "Open for white list":
                    var openRdpForWhiteListTimer = new RunningTimerModel(duration, (t) => ActiveTimers.Remove(t), _rdpService.OpenRdpForWhiteList, "Open for white list");
                    ActiveTimers.Add(openRdpForWhiteListTimer);
                    break;
                case "Open for local computers":
                    var openRdpForLocalComputersTimer = new RunningTimerModel(duration, (t) => ActiveTimers.Remove(t), _rdpService.OpenRdpForLocalComputers, "Open for local computers");
                    ActiveTimers.Add(openRdpForLocalComputersTimer);
                    break;
                case "Open for local computers and for white list":
                    var openRdpForLocalComputersAndForWhiteListTimer = new RunningTimerModel(duration, (t) => ActiveTimers.Remove(t), _rdpService.OpenRdpForLocalComputersAndForWhiteList, "Open for local computers and for white list");
                    ActiveTimers.Add(openRdpForLocalComputersAndForWhiteListTimer);
                    break;
                case "Open for all":
                    var openRdpForAllTimer = new RunningTimerModel(duration, (t) => ActiveTimers.Remove(t), _rdpService.OpenRdpForAll, "Open for all");
                    ActiveTimers.Add(openRdpForAllTimer);
                    /*_ = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.Parse($"{CountDownHour}:{CountDownMinute}"));
                        _rdpService.OpenRdpForAll();
                        Console.WriteLine("Changed to: 192.168.0.0-192.168.255.255");
                        MessageBox.Show("Changed to: 192.168.0.0-192.168.255.255");
                    });*/
                    break;
                case "Close for all":
                    var closeRdpForAllTimer = new RunningTimerModel(duration, (t) => ActiveTimers.Remove(t), _rdpService.CloseRdpForAll, "Close for all");
                    ActiveTimers.Add(closeRdpForAllTimer);
                    break;

                default:
                    Debug.WriteLine("Something went wrong");
                    break;
            }
        }

        private void RunProgramByPort(int port)
        {
            string protocolTcp = "6"; // TCP protocol number
            bool didFound = false;

            var fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            foreach (INetFwRule rule in fwPolicy2.Rules)
            {
                // נוודא שזה חוק TCP עם פורט תואם
                if (rule.Protocol.ToString() == protocolTcp &&
                    rule.LocalPorts != null &&
                    rule.LocalPorts.Split(',').Any(p => p.Trim() == port.ToString()))
                {
                    didFound = true;
                    Console.WriteLine($"Rule found: {rule.Name}");
                    Console.WriteLine($"Current RemoteAddresses: {rule.RemoteAddresses}");

                    if (rule.RemoteAddresses == "*")
                    {
                        rule.RemoteAddresses = "192.168.0.0-192.168.255.255";
                        Console.WriteLine("Changed to: 192.168.0.0-192.168.255.255");
                        MessageBox.Show("Changed to: 192.168.0.0-192.168.255.255");
                    }
                    else
                    {
                        rule.RemoteAddresses = "*";
                        Console.WriteLine("Changed to: * (Any)");

                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(TimeSpan.Parse($"{CountDownHour}:{CountDownMinute}"));
                            rule.RemoteAddresses = "192.168.0.0-192.168.255.255";
                            Console.WriteLine("Changed to: 192.168.0.0-192.168.255.255");
                            MessageBox.Show("Changed to: 192.168.0.0-192.168.255.255");
                        });
                    }

                    break;
                }
            }

            if (!didFound)
                MessageBox.Show($"No firewall rule found for port {port}");
        }

    }
}

using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GraphicRdpScopeToggler.Models
{
    public class RunningTimerModel : BindableBase
    {
        private TimeSpan timeLeft;
        public TimeSpan TimeLeft
        {
            get => timeLeft;
            set => SetProperty(ref timeLeft, value);
        }

        public ICommand CancelCommand { get; set; }

        private DispatcherTimer timer;
        private readonly Action<RunningTimerModel> onCancel;

        public RunningTimerModel(TimeSpan duration, Action<RunningTimerModel> onCancel, Action onFinish)
        {
            TimeLeft = duration;
            this.onCancel = onCancel;

            CancelCommand = new DelegateCommand(() =>
            {
                timer.Stop();
                onCancel?.Invoke(this);
            });

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (s, e) =>
            {
                if (TimeLeft.TotalSeconds <= 1)
                {
                    onFinish.Invoke();
                    timer.Stop();
                    onCancel?.Invoke(this);
                }
                else
                {
                    TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
                }
            };
            timer.Start();
        }
    }

}

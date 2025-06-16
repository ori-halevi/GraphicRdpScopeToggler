using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GraphicRdpScopeToggler.Models
{
    public class RunningTimerModel : BindableBase
    {
        private string taskTitle;
        public string TaskTitle
        {
            get { return taskTitle; }
            set { SetProperty(ref taskTitle, value); }
        }

        private TimeSpan timeLeft;
        public TimeSpan TimeLeft
        {
            get => timeLeft;
            set => SetProperty(ref timeLeft, value);
        }

        public ICommand CancelCommand { get; set; }

        private readonly Action<RunningTimerModel> onCancel;
        private readonly Action onFinish;
        private CancellationTokenSource cts;

        public RunningTimerModel(TimeSpan duration, Action<RunningTimerModel> onCancel, Action onFinish, string taskTitle)
        {
            TaskTitle = taskTitle;
            TimeLeft = duration;
            this.onCancel = onCancel;
            this.onFinish = onFinish;

            cts = new CancellationTokenSource();

            CancelCommand = new DelegateCommand(() =>
            {
                cts.Cancel();
                onCancel?.Invoke(this);
            });

            StartTimerAsync();
        }

        private async void StartTimerAsync()
        {
            try
            {
                while (TimeLeft.TotalSeconds > 1 && !cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000, cts.Token);

                    // העדכון חייב לרוץ על ה־UI Thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
                    });
                }

                if (!cts.Token.IsCancellationRequested)
                {
                    onFinish?.Invoke();

                    // סיום של האובייקט (כמו הסרה מרשימה וכו')
                    onCancel?.Invoke(this);
                }
            }
            catch (TaskCanceledException)
            {
                // התעלמות מביטול מכוון
            }
        }
    }
}


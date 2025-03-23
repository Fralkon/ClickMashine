using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine
{
    public abstract class MyTask
    {
        private readonly CancellationTokenSource _cts = new();
        private Task? _task;

        protected bool CheckCancellationToken() => _cts.Token.IsCancellationRequested;

        public void Start()
        {
            if (_task is { Status: TaskStatus.Running })
            {
                Console.WriteLine("[Warning] Задача уже запущена.");
                return;
            }

            _task = Task.Run(() => StartSurf(), _cts.Token);
        }

        protected abstract void StartSurf();

        public void Stop()
        {
            if (_task is not { Status: TaskStatus.Running })
            {
                Console.WriteLine("[Warning] Задача не запущена или уже остановлена.");
                return;
            }

            _cts.Cancel();
        }
    }
    public abstract class BrowseTask
    {
        private readonly CancellationTokenSource _cts = new();
        private Task? _task;

        protected bool CheckCancellationToken() => _cts.Token.IsCancellationRequested;

        public void Start()
        {
            if (_task is { Status: TaskStatus.Running })
            {
                Console.WriteLine("[Warning] Задача уже запущена.");
                return;
            }

            _task = Task.Run(() => StartSurfAsync(_cts.Token), _cts.Token);
        }

        protected abstract Task StartSurfAsync(CancellationToken token);

        public void Stop()
        {
            if (_task is not { Status: TaskStatus.Running })
            {
                Console.WriteLine("[Warning] Задача не запущена или уже остановлена.");
                return;
            }

            _cts.Cancel();
        }
    }
}

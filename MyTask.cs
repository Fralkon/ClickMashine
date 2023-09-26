using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine_11
{
    public abstract class MyTask
    {
        protected CancellationToken cancellationToken = new CancellationToken();
        Task task;
        public MyTask()
        {
            task = new Task(StartSurf);
        }
        protected bool CheckCancellationToken()
        {
            return cancellationToken.IsCancellationRequested;
        }
        public void Start()
        {
            task.Start();
        }
        protected abstract void StartSurf();
        public void Stop()
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}

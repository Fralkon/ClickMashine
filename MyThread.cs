using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine
{
    abstract class MyThread
    {
        protected CancellationToken cancellationToken = new CancellationToken();
        Thread thread;
        public MyThread()
        {
            thread = new Thread(StartSurf);
        }
        protected bool CheckCancellationToken()
        {
            return cancellationToken.IsCancellationRequested;
        }
        public void Start()
        {
            thread.Start();
        }
        protected abstract void StartSurf();
        public void Stop()
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine
{
    class MyThread
    {
        protected CancellationToken cancellationToken = new CancellationToken();
        public MyThread()
        {
        }
        protected bool CheckCancellationToken()
        {
            return cancellationToken.IsCancellationRequested;
        }
        public void Start()
        {
            Task.Run(() => StartSurf());
        }
        protected virtual void StartSurf()
        {

        }
        public void Stop()
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}

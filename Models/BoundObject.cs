using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine.Models
{
    public class BoundObject
    {
        private EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        string Text { get; set; }
        public void ResetEvent() =>
            EventWaitHandle.Reset();
        public void Event(string text)
        {
            Text = text;
            EventWaitHandle.Set();
        }
        public string GetValue(int time = 20)
        {
            if (EventWaitHandle.WaitOne(time * 1000))
                return Text;
            else
                return "error";
        }
    }
}

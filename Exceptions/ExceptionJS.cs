using ClickMashine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine.Exceptions
{
    class ExceptionJS : Exception
    {
        public EnumTypeSite TypeSite { get; set; }
        string JS;
        string mess;
        public ExceptionJS(EnumTypeSite typeSite, string js, string message) : base()
        {
            TypeSite = typeSite;
            JS = js;
            mess = message;
        }
        public new string Message { get { return $"Site : {TypeSite}\nJS : {JS}\nMessage : {mess}"; } }
    }
}

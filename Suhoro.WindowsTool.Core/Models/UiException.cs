using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.Core.Models
{
    public class UiException : Exception
    {
        public string UiMessage { get; set; }
        public UiException(string? uiMessage, string? message=null) : base(message??uiMessage)
        {
            UiMessage = uiMessage;
        }
    }
}

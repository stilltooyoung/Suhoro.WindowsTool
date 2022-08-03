using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Suhoro.WindowsTool.Core.Utils
{
    public class GeneralCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Func<object?,bool>? FuncCanExecute;
        public Action<object?>? ActionExecute;
        public GeneralCommand(Action<object?>? actionExecute=null, Func<object?,bool>? funcCanExecute=null) 
        { 
            ActionExecute = actionExecute;
            FuncCanExecute = funcCanExecute;
        }

        public virtual bool CanExecute(object? parameter)
        {
            return FuncCanExecute?.Invoke(parameter) ?? true;
        }

        public virtual void Execute(object? parameter)
        {
            ActionExecute?.Invoke(parameter);
        }
    }

    public class GeneralCommand<TOtherParam> : GeneralCommand
    {
        public TOtherParam OtherParam { get; set; }
        public new Action<object?, TOtherParam>? ActionExecute { get; }
        public new Func<object?, TOtherParam,bool>? FuncCanExecute { get; }

        public GeneralCommand(TOtherParam otherParam,
            Action<object?, TOtherParam>? actionExecute=null,
            Func<object?, TOtherParam,bool>? funcCanExecute =null)
        {
            OtherParam = otherParam;
            ActionExecute = actionExecute;
            FuncCanExecute = funcCanExecute ;
        }

        public override bool CanExecute(object? parameter)
        {
            return FuncCanExecute?.Invoke(parameter, OtherParam) ?? base.CanExecute(parameter);
        }
        public override void Execute(object? parameter)
        {
            ActionExecute?.Invoke(parameter, OtherParam);
        }
    }
}

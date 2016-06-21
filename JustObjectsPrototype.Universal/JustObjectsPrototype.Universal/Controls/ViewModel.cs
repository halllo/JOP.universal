using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace JustObjectsPrototype.Universal.Controls
{
	public abstract class ViewModel : INotifyPropertyChanged
	{
		public void RaiseChanged(string member)
		{
			Changed(member);
		}

		protected void Changed([CallerMemberName]string member = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
		}

		protected virtual void Changed<T>(Expression<Func<T>> selectorExpression)
		{
			if (selectorExpression == null) throw new ArgumentNullException("selectorExpression");

			MemberExpression body = selectorExpression.Body as MemberExpression;
			if (body == null) throw new ArgumentException("The body must be a member expression");

			Changed(body.Member.Name);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class Command : ICommand
	{
		Action _Execute;
		Func<bool> _CanExecute;

		public Command(Action execute, Func<bool> canExecute = null)
		{
			_Execute = execute;
			_CanExecute = canExecute ?? (() => true);
		}

		public bool CanExecute(object parameter)
		{
			return _CanExecute();
		}

		public void Execute(object parameter)
		{
			_Execute();
		}

		public event EventHandler CanExecuteChanged;
		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}
	}
}

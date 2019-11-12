using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace WpfApp1.Command 
{
    /// <summary>
    /// An <see cref="T:System.Windows.Input.ICommand" /> whose delegates can be attached for <see
    /// cref="M:MVVM.ViewModel.DelegateCommandBase.Execute(System.Object)" /> and <see
    /// cref="M:MVVM.ViewModel.DelegateCommandBase.CanExecute(System.Object)" />. It also
    /// implements the <see cref="T:MVVM.ViewModel.IActiveAware" /> interface, which is
    /// useful when registering this command in a <see cref="!:CompositeCommand" /> that monitors
    /// command's activity.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand, IActiveAware
    {
        private readonly Action<object> executeMethod;

        private readonly Func<object, bool> canExecuteMethod;

        private bool _isActive;

        private List<WeakReference> _canExecuteChangedHandlers;

        /// <summary>
        /// Gets or sets a value indicating whether the object is active.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the object is active; otherwise <see langword="false" />.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this._isActive;
            }
            set
            {
                if (this._isActive != value)
                {
                    this._isActive = value;
                    this.OnIsActiveChanged();
                }
            }
        }

        /// <summary>
        /// Createse a new instance of a <see cref="T:MVVM.ViewModel.DelegateCommandBase"
        /// />, specifying both the execute action and the can execute function.
        /// </summary>
        /// <param name="executeMethod">
        /// The <see cref="T:System.Action" /> to execute when <see
        /// cref="M:System.Windows.Input.ICommand.Execute(System.Object)" /> is invoked.
        /// </param>
        /// <param name="canExecuteMethod">
        /// The <see cref="T:System.Func`2" /> to invoked when <see
        /// cref="M:System.Windows.Input.ICommand.CanExecute(System.Object)" /> is invoked.
        /// </param>
        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                return;
                //throw new ArgumentNullException("executeMethod", Resources.DelegateCommandDelegatesCannotBeNull);
            }
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Determines if the command can execute with the provided parameter by invoing the <see
        /// cref="T:System.Func`2" /> supplied during construction.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to use when determining if this command can execute.
        /// </param>
        /// <returns>
        /// Returns <see langword="true" /> if the command can execute. <see langword="False" /> otherwise.
        /// </returns>
        protected bool CanExecute(object parameter)
        {
            if (this.canExecuteMethod == null)
            {
                return true;
            }
            return this.canExecuteMethod(parameter);
        }

        /// <summary>
        /// Executes the command with the provided parameter by invoking the <see
        /// cref="T:System.Action`1" /> supplied during construction.
        /// </summary>
        /// <param name="parameter"> </param>
        protected void Execute(object parameter)
        {
            this.executeMethod(parameter);
        }

        /// <summary>
        /// Raises <see cref="E:System.Windows.Input.ICommand.CanExecuteChanged" /> on the UI thread
        /// so every command invoker can requery <see
        /// cref="M:System.Windows.Input.ICommand.CanExecute(System.Object)" /> to check if the <see
        /// cref="!:CompositeCommand" /> can execute.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            WeakEventHandlerManager.CallWeakReferenceHandlers(this, this._canExecuteChangedHandlers);
        }

        /// <summary>
        /// This raises the <see
        /// cref="E:MVVM.ViewModel.DelegateCommandBase.IsActiveChanged" /> event.
        /// </summary>
        protected virtual void OnIsActiveChanged()
        {
            EventHandler isActiveChangedHandler = this.IsActiveChanged;
            if (isActiveChangedHandler != null)
            {
                isActiveChangedHandler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises <see cref="E:MVVM.ViewModel.DelegateCommandBase.CanExecuteChanged" />
        /// on the UI thread so every command invoker can requery to check if the command can
        /// execute. <remarks>Note that this will trigger the execution of <see
        /// cref="M:MVVM.ViewModel.DelegateCommandBase.CanExecute(System.Object)" /> once
        /// for each invoker.</remarks>
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        bool System.Windows.Input.ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter);
        }

        void System.Windows.Input.ICommand.Execute(object parameter)
        {
            this.Execute(parameter);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute. You
        /// must keep a hard reference to the handler to avoid garbage collection and unexpected
        /// results. See remarks for more information.
        /// </summary>
        /// <remarks>
        /// When subscribing to the <see cref="E:System.Windows.Input.ICommand.CanExecuteChanged" />
        /// event using code (not when binding using XAML) will need to keep a hard reference to the
        /// event handler. This is to prevent garbage collection of the event handler because the
        /// command implements the Weak Event pattern so it does not have a hard reference to this
        /// handler. An example implementation can be seen in the CompositeCommand and
        /// CommandBehaviorBase classes. In most scenarios, there is no reason to sign up to the
        /// CanExecuteChanged event directly, but if you do, you are responsible for maintaining the reference.
        /// </remarks>
        /// <example>
        /// The following code holds a reference to the event handler. The myEventHandlerReference
        /// value should be stored in an instance member to avoid it from being garbage collected.
        /// <code> EventHandler myEventHandlerReference = new
        /// EventHandler(this.OnCanExecuteChanged); command.CanExecuteChanged +=
        /// myEventHandlerReference; </code>
        /// </example>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                WeakEventHandlerManager.AddWeakReferenceHandler(ref this._canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                WeakEventHandlerManager.RemoveWeakReferenceHandler(this._canExecuteChangedHandlers, value);
            }
        }

        /// <summary>
        /// Fired if the <see cref="P:MVVM.ViewModel.DelegateCommandBase.IsActive" />
        /// property changes.
        /// </summary>
        public virtual event EventHandler IsActiveChanged;
    }
}
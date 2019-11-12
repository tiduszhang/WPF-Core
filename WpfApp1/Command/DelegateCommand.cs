using System;

namespace WpfApp1.Command
{
    /// <summary>
    /// An <see cref="T:System.Windows.Input.ICommand" /> whose delegates do not take any parameters for <see cref="M:MVVM.ViewModel.DelegateCommand.Execute" /> and <see cref="M:MVVM.ViewModel.DelegateCommand.CanExecute" />.
    /// </summary>
    /// <seealso cref="T:MVVM.ViewModel.DelegateCommandBase" />
    /// <seealso cref="T:MVVM.ViewModel.DelegateCommand`1" />
    public class DelegateCommand : DelegateCommandBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="T:MVVM.ViewModel.DelegateCommand" /> with the <see cref="T:System.Action" /> to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The <see cref="T:System.Action" /> to invoke when <see cref="M:System.Windows.Input.ICommand.Execute(System.Object)" /> is called.</param>
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="T:MVVM.ViewModel.DelegateCommand" /> with the <see cref="T:System.Action" /> to invoke on execution
        /// and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="executeMethod">The <see cref="T:System.Action" /> to invoke when <see cref="M:System.Windows.Input.ICommand.Execute(System.Object)" /> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="T:System.Func`1" /> to invoke when <see cref="M:System.Windows.Input.ICommand.CanExecute(System.Object)" /> is called</param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base((object o) => executeMethod(), (object o) => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                return;
                //throw new ArgumentNullException("executeMethod", Resources.DelegateCommandDelegatesCannotBeNull);
            }
        }

        /// <summary>
        /// Determines if the command can be executed.
        /// </summary>
        /// <returns>Returns <see langword="true" /> if the command can execute,otherwise returns <see langword="false" />.</returns>
        public bool CanExecute()
        {
            return base.CanExecute(null);
        }

        /// <summary>
        ///  Executes the command.
        /// </summary>
        public void Execute()
        {
            base.Execute(null);
        }
    }

    /// <summary>
    /// An <see cref="T:System.Windows.Input.ICommand" /> whose delegates can be attached for <see cref="M:MVVM.ViewModel.DelegateCommand`1.Execute(`0)" /> and <see cref="M:MVVM.ViewModel.DelegateCommand`1.CanExecute(`0)" />.
    /// It also implements the <see cref="T:MVVM.ViewModel.IActiveAware" /> interface, which is useful when registering this command in a <see cref="!:CompositeCommand" /> that monitors command's activity.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    /// <remarks>
    /// The constructor deliberately prevent the use of value types.
    /// Because ICommand takes an object, having a value type for T would cause unexpected behavior when CanExecute(null) is called during XAML initialization for command bindings.
    /// Using default(T) was considered and rejected as a solution because the implementor would not be able to distinguish between a valid and defaulted values.
    /// <para />
    /// Instead, callers should support a value type by using a nullable value type and checking the HasValue property before using the Value property.
    /// <example>
    ///     <code>
    /// public MyClass()
    /// {
    ///     this.submitCommand = new DelegateCommand&lt;int?&gt;(this.Submit, this.CanSubmit);
    /// }
    /// private bool CanSubmit(int? customerId)
    /// {
    ///     return (customerId.HasValue &amp;&amp; customers.Contains(customerId.Value));
    /// }
    ///     </code>
    /// </example>
    /// </remarks>
    public class DelegateCommand<T> : DelegateCommandBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="T:MVVM.ViewModel.DelegateCommand`1" />.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="M:MVVM.ViewModel.DelegateCommand`1.CanExecute(`0)" /> will always return true.</remarks>
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, (T o) => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="T:MVVM.ViewModel.DelegateCommand`1" />.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
        /// <exception cref="T:System.ArgumentNullException">When both <paramref name="executeMethod" /> and <paramref name="canExecuteMethod" /> ar <see langword="null" />.</exception>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base((object o) => executeMethod((T)o), (object o) => canExecuteMethod((T)o))
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                return;
                //throw new ArgumentNullException("executeMethod", Resources.DelegateCommandDelegatesCannotBeNull);
            }
            Type genericType = typeof(T);
            if (genericType.IsValueType && (!genericType.IsGenericType || !typeof(Nullable<>).IsAssignableFrom(genericType.GetGenericTypeDefinition())))
            {
                return;
                //throw new InvalidCastException(Resources.DelegateCommandInvalidGenericPayloadType);
            }
        }

        /// <summary>
        /// Determines if the command can execute by invoked the <see cref="T:System.Func`2" /> provided during construction.
        /// </summary>
        /// <param name="parameter">Data used by the command to determine if it can execute.</param>
        /// <returns>
        /// <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        /// <summary>
        /// Executes the command and invokes the <see cref="T:System.Action`1" /> provided during construction.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public void Execute(T parameter)
        {
            base.Execute(parameter);
        }
    }
}
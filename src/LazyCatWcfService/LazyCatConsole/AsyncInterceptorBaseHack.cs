using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Hack by ndrwrbgs for issue #25 of JSkimming/Castle.Core.AsyncInterceptor
	/// https://github.com/JSkimming/Castle.Core.AsyncInterceptor/issues/25#issuecomment-342002270
	/// </summary>
	public abstract class AsyncInterceptorBaseHack : AsyncInterceptorBase, IAsyncInterceptor
	{
		private readonly FieldInfo currentInterceptorIndexField;

		protected AsyncInterceptorBaseHack()
		{
			// Because awaits will make it decrease it's nice little counter before it's done, we wrap that little counter up <3
			// Little counters need love too
			this.currentInterceptorIndexField = typeof(AbstractInvocation).GetField(
				"currentInterceptorIndex",
				BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public virtual void InterceptSynchronous(IInvocation invocation)
		{
			invocation.Proceed();
		}

		protected sealed override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
		{
			// Artificially increment currentInterceptorIndex, we don't actually want to increment it but we DO want to prevent the decrement the base class performs
			int currentValue = (int)this.currentInterceptorIndexField.GetValue(invocation);
			this.currentInterceptorIndexField.SetValue(invocation, (object)(currentValue + 1));

			try
			{
				// base class will decrement currentInterceptorIndex right after the first await returns control, ensure that is immediate with Task.Yield
				await Task.Yield();
				await InterceptAsyncCore(invocation, proceed);
			}
			finally
			{
				// Decrement currentInterceptorIndex (reset to what it was before)
				this.currentInterceptorIndexField.SetValue(invocation, currentValue);
			}
		}

		protected sealed override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
		{
			// Artificially increment currentInterceptorIndex, we don't actually want to increment it but we DO want to prevent the decrement the base class performs
			int currentValue = (int)this.currentInterceptorIndexField.GetValue(invocation);
			this.currentInterceptorIndexField.SetValue(invocation, (object)(currentValue + 1));

			try
			{
				// base class will decrement currentInterceptorIndex right after the first await returns control, ensure that is immediate with Task.Yield
				await Task.Yield();
				return await InterceptAsyncCore(invocation, proceed);
			}
			finally
			{
				// Decrement currentInterceptorIndex (reset to what it was before)
				this.currentInterceptorIndexField.SetValue(invocation, currentValue);
			}
		}

		protected abstract Task InterceptAsyncCore(IInvocation invocation, Func<IInvocation, Task> proceed);

		protected abstract Task<TResult> InterceptAsyncCore<TResult>(
			IInvocation invocation,
			Func<IInvocation, Task<TResult>> proceed);
	}
}

﻿using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace LazyCatConsole
{
	/// <summary>
	/// Intercepts calls to slim service interface and invokes corresponding methods on
	/// the real client.
	/// </summary>
	/// <typeparam name="TChannel">Service interface</typeparam>
	internal class SlimServiceClientDispatcher<TChannel> : IInterceptor where TChannel : class
	{
		readonly ClientBase<TChannel> m_RealClient;

		public SlimServiceClientDispatcher(ClientBase<TChannel> realClient)
		{
			m_RealClient = realClient ?? throw new ArgumentNullException(nameof(realClient));
		}

		public void Intercept(IInvocation invocation)
		{
			Type targetType = invocation.Method.DeclaringType;
			Type realType = m_RealClient.GetType();
			bool assignable = targetType.IsAssignableFrom(realType);
			if (!assignable)
			{
				throw new InvalidProgramException("Something went wrong :(");
			}

			Type realClientInterface = realType.GetInterface(targetType.FullName);
			MethodInfo[] realMethods = realClientInterface.GetMethods();
			var methodToInvoke = realMethods.First(m => m.Name.Equals(invocation.Method.Name));

			try
			{
				var methodResult = methodToInvoke.Invoke(m_RealClient, invocation.Arguments);
				invocation.ReturnValue = methodResult;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}
	}
}
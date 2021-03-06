﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Configuration;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Cqrs.Ninject.Configuration
{
	/// <summary>
	/// An implementation of <see cref="IDependencyResolver"/> using Ninject
	/// </summary>
	public class NinjectDependencyResolver : IDependencyResolver
	{
		/// <summary>
		/// The current instance of the <see cref="IDependencyResolver"/>.
		/// </summary>
		public static IDependencyResolver Current { get; protected set; }

		/// <summary>
		/// The inner Ninject <see cref="IKernel"/> used by this instance.
		/// </summary>
		protected IKernel Kernel { get; private set; }

		/// <summary>
		/// A collection of <see cref="INinjectModule"/> instances to load when we call <see cref="PrepareKernel"/>
		/// </summary>
		public static IList<INinjectModule> ModulesToLoad = new List<INinjectModule>();

		/// <summary>
		/// A user supplied <see cref="Func{TResult}"/> that will be called during <see cref="Start"/> to create and populate <see cref="Current"/>.
		/// </summary>
		public static Func<IKernel, NinjectDependencyResolver> DependencyResolverCreator { get; set; }

		/// <summary>
		/// Instantiates a new instance of <see cref="NinjectDependencyResolver"/>
		/// </summary>
		/// <param name="kernel"></param>
		public NinjectDependencyResolver(IKernel kernel)
		{
			Kernel = kernel;
			BindDependencyResolver();
		}

		protected virtual void BindDependencyResolver()
		{
			bool isDependencyResolverBound = Kernel.GetBindings(typeof(IDependencyResolver)).Any();
			if (!isDependencyResolverBound)
			{
				Kernel.Bind<IDependencyResolver>()
					.ToConstant(this)
					.InSingletonScope();
			}
		}

		/// <summary>
		/// Starts the <see cref="NinjectDependencyResolver"/>
		/// </summary>
		/// <remarks>
		/// this exists to the static constructor can be triggered.
		/// </remarks>
		public static void Start(IKernel kernel = null, bool prepareProvidedKernel = false)
		{
			if (kernel == null)
			{
				kernel = new StandardKernel();
				prepareProvidedKernel = true;
			}

			if (DependencyResolverCreator != null)
				Current = DependencyResolverCreator(kernel);
			else
				Current = new NinjectDependencyResolver(kernel);

			if (prepareProvidedKernel)
				PrepareKernel(kernel);
		}

		/// <summary>
		/// Calls <see cref="IKernel.Load(IEnumerable{INinjectModule})"/> passing in <see cref="ModulesToLoad"/>
		/// </summary>
		/// <param name="kernel">The <see cref="IKernel"/> the <see cref="ModulesToLoad"/> will be loaded into.</param>
		public static void PrepareKernel(IKernel kernel)
		{
			kernel.Load
			(
				ModulesToLoad
			);
		}

		/// <summary>
		/// Calls <see cref="IResolutionRoot.Resolve"/>
		/// </summary>
		public virtual T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}

		/// <summary>
		/// Calls <see cref="IResolutionRoot.Resolve"/>
		/// </summary>
		public virtual object Resolve(Type serviceType)
		{
			return Kernel.Resolve(Kernel.CreateRequest(serviceType, null, new Parameter[0], true, true)).SingleOrDefault();
		}
	}
}
﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Cqrs.Akka.Configuration;
using Cqrs.Akka.Domain;
using Cqrs.Domain;
using Cqrs.Domain.Factories;
using Cqrs.Ninject.Configuration;
using Ninject;

namespace Cqrs.Ninject.Akka
{
	public class AkkaNinjectDependencyResolver
		: NinjectDependencyResolver
		, IAkkaAggregateResolver
		, IAkkaSagaResolver
		, IHandlerResolver
	{
		protected global::Akka.DI.Ninject.NinjectDependencyResolver RawAkkaNinjectDependencyResolver { get; set; }

		protected ActorSystem AkkaSystem { get; private set; }

		protected IDictionary<Type, IActorRef> AkkaActors { get; private set; }

		protected IAggregateFactory AggregateFactory { get; private set; }

		public AkkaNinjectDependencyResolver(IKernel kernel, ActorSystem system)
			: base(kernel)
		{
			RawAkkaNinjectDependencyResolver = new global::Akka.DI.Ninject.NinjectDependencyResolver(kernel, AkkaSystem = system);
			AkkaActors = new ConcurrentDictionary<Type, IActorRef>();
			AggregateFactory = Resolve<IAggregateFactory>();
		}

		protected override void BindDependencyResolver()
		{
			bool isDependencyResolverBound = Kernel.GetBindings(typeof(Cqrs.Configuration.IDependencyResolver)).Any();
			if (isDependencyResolverBound)
				Kernel.Unbind<Cqrs.Configuration.IDependencyResolver>();
			Kernel.Bind<Cqrs.Configuration.IDependencyResolver>()
				.ToConstant(this)
				.InSingletonScope();

			isDependencyResolverBound = Kernel.GetBindings(typeof(IAkkaAggregateResolver)).Any();
			if (!isDependencyResolverBound)
			{
				Kernel.Bind<IAkkaAggregateResolver>()
					.ToConstant(this)
					.InSingletonScope();
			}
		}

		/// <summary>
		/// Starts the <see cref="AkkaNinjectDependencyResolver"/>
		/// </summary>
		/// <remarks>
		/// this exists to the static constructor can be triggered.
		/// </remarks>
		public new static void Start(IKernel kernel = null, bool prepareProvidedKernel = false)
		{
			// Create the ActorSystem and Dependency Resolver
			ActorSystem system = ActorSystem.Create("Cqrs");

			Func<IKernel, NinjectDependencyResolver> originalDependencyResolverCreator = DependencyResolverCreator;
			Func<IKernel, NinjectDependencyResolver> dependencyResolverCreator = container => new AkkaNinjectDependencyResolver(container, system);
			if (originalDependencyResolverCreator == null)
				DependencyResolverCreator = dependencyResolverCreator;
			else
				DependencyResolverCreator = container =>
				{
					originalDependencyResolverCreator(container);
					return dependencyResolverCreator(container);
				};

			NinjectDependencyResolver.Start(kernel, prepareProvidedKernel);
		}

		public static void Stop()
		{
			var di = Current as AkkaNinjectDependencyResolver;
			if (di != null)
				di.AkkaSystem.Shutdown();
		}

		#region Overrides of NinjectDependencyResolver

		public override object Resolve(Type serviceType)
		{
			return Resolve(serviceType, null);
		}

		#endregion

		#region Implementation of IAkkaAggregateResolver

		public virtual IActorRef ResolveActor<TAggregate, TAuthenticationToken>(Guid rsn)
			where TAggregate : IAggregateRoot<TAuthenticationToken>
		{
			return (IActorRef)AkkaResolve(typeof(TAggregate), rsn, true);
		}

		public IActorRef ResolveActor<T>()
		{
			return (IActorRef)AkkaResolve(typeof(T), null, true);
		}

		#endregion

		#region Implementation of IAkkaSagaResolver

		IActorRef IAkkaSagaResolver.ResolveActor<TSaga, TAuthenticationToken>(Guid rsn)
		{
			return ResolveSagaActor<TSaga, TAuthenticationToken>(rsn);
		}

		public virtual IActorRef ResolveSagaActor<TSaga, TAuthenticationToken>(Guid rsn)
			where TSaga : ISaga<TAuthenticationToken>
		{
			return (IActorRef)AkkaResolve(typeof(TSaga), rsn, true);
		}

		#endregion

		protected virtual object RootResolve(Type serviceType)
		{
			return base.Resolve(serviceType);
		}

		public virtual object Resolve(Type serviceType, object rsn)
		{
			return AkkaResolve(serviceType, rsn);
		}

		public virtual object AkkaResolve(Type serviceType, object rsn, bool isAForcedActorSearch = false)
		{
			IActorRef actorReference;
			try
			{
				if (AkkaActors.TryGetValue(serviceType, out actorReference))
					return actorReference;
				if (!isAForcedActorSearch)
					return base.Resolve(serviceType);
			}
			catch (ActivationException) { throw; }
			catch ( /*ActorInitialization*/Exception) { }

			Props properties;
			Type typeToTest = serviceType;
			while (typeToTest != null)
			{
				Type[] types = typeToTest.GenericTypeArguments;
				if (types.Length == 1)
				{
					Type aggregateType = typeof (AkkaAggregateRoot<>).MakeGenericType(typeToTest.GenericTypeArguments.Single());
					if (typeToTest == aggregateType)
					{
						typeToTest = aggregateType;
						break;
					}
					Type sagaType = typeof (AkkaSaga<>).MakeGenericType(typeToTest.GenericTypeArguments.Single());
					if (typeToTest == sagaType)
					{
						typeToTest = sagaType;
						break;
					}
				}
				typeToTest = typeToTest.BaseType;
			}

			// This sorts out an out-of-order binder issue
			if (AggregateFactory == null)
				AggregateFactory = Resolve<IAggregateFactory>();

			if (typeToTest == null || !(typeToTest).IsAssignableFrom(serviceType))
				properties = Props.Create(() => (ActorBase)RootResolve(serviceType));
			else
				properties = Props.Create(() => (ActorBase) AggregateFactory.Create(serviceType, rsn as Guid?, false));
			string actorName = serviceType.FullName.Replace("`", string.Empty);
			int index = actorName.IndexOf("[[", StringComparison.Ordinal);
			if (index > -1)
				actorName = actorName.Substring(0, index);
			actorReference = AkkaSystem.ActorOf(properties, string.Format("{0}~{1}", actorName, rsn));
			AkkaActors.Add(serviceType, actorReference);
			return actorReference;
		}
	}
}
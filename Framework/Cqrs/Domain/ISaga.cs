﻿#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cqrs.Commands;
using Cqrs.Events;

namespace Cqrs.Domain
{
	/// <summary>
	/// An independent component that reacts to domain <see cref="IEvent{TAuthenticationToken}"/> in a cross-<see cref="IAggregateRoot{TAuthenticationToken}"/>, eventually consistent manner. Time can also be a trigger. A <see cref="Saga{TAuthenticationToken}"/> can sometimes be purely reactive, and sometimes represent workflows.
	/// 
	/// From an implementation perspective, a <see cref="Saga{TAuthenticationToken}"/> is a state machine that is driven forward by incoming <see cref="IEvent{TAuthenticationToken}"/> (which may come from many <see cref="AggregateRoot{TAuthenticationToken}"/> or other <see cref="Saga{TAuthenticationToken}"/>). Some states will have side effects, such as sending <see cref="ICommand{TAuthenticationToken}"/>, talking to external web services, or sending emails.
	/// </summary>
	/// <remarks>
	/// Isn't a <see cref="Saga{TAuthenticationToken}"/> just leaked domain logic?
	/// No.
	/// A <see cref="Saga{TAuthenticationToken}"/> can doing things that no individual <see cref="AggregateRoot{TAuthenticationToken}"/> can sensibly do. Thus, it's not a logic leak since the logic didn't belong in an <see cref="AggregateRoot{TAuthenticationToken}"/> anyway. Furthermore, we're not breaking encapsulation in any way, since <see cref="Saga{TAuthenticationToken}"/> operate with <see cref="ICommand{TAuthenticationToken}"/> and <see cref="IEvent{TAuthenticationToken}"/>, which are part of the public API.
	/// 
	/// How can I make my <see cref="Saga{TAuthenticationToken}"/> react to an <see cref="IEvent{TAuthenticationToken}"/> that did not happen?
	/// The <see cref="Saga{TAuthenticationToken}"/>, besides reacting to domain <see cref="IEvent{TAuthenticationToken}"/>, can be "woken up" by recurrent internal alarms. Implementing such alarms is easy. See cron in Unix, or triggered WebJobs in Azure for examples.
	/// 
	/// How does the <see cref="Saga{TAuthenticationToken}"/> interact with the write side?
	/// By sending an <see cref="ICommand{TAuthenticationToken}"/> to it.
	/// </remarks>
	public interface ISaga<TAuthenticationToken>
	{
		[DataMember]
		Guid Id { get; }

		[DataMember]
		int Version { get; }

		IEnumerable<ISagaEvent<TAuthenticationToken>> GetUncommittedChanges();

		void MarkChangesAsCommitted();

		void LoadFromHistory(IEnumerable<ISagaEvent<TAuthenticationToken>> history);
	}
}
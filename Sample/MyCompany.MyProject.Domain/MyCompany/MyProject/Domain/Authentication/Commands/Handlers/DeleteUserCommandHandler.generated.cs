﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#region Copyright
// -----------------------------------------------------------------------
// <copyright company="cdmdotnet Limited">
//     Copyright cdmdotnet Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
#endregion
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Cqrs.Commands;
using Cqrs.Domain;

namespace MyCompany.MyProject.Domain.Authentication.Commands.Handlers
{
	[GeneratedCode("CQRS UML Code Generator", "1.500.523.412")]
	public  partial class DeleteUserCommandHandler : ICommandHandler<Cqrs.Authentication.ISingleSignOnToken, DeleteUserCommand>
	{
		protected IUnitOfWork<Cqrs.Authentication.ISingleSignOnToken> UnitOfWork { get; private set; }

		public DeleteUserCommandHandler(IUnitOfWork<Cqrs.Authentication.ISingleSignOnToken> unitOfWork)
		{
			UnitOfWork = unitOfWork;
		}

		#region Implementation of ICommandHandler<in DeleteUserCommand>

		public void Handle(DeleteUserCommand command)
		{
			User item = null;
			OnDeleteUser(command, ref item);
			if (item == null)
				item = UnitOfWork.Get<User>(command.Rsn);
			item.DeleteUser();
			OnDeletedUser(command, item);
			OnCommit(command, item);
			UnitOfWork.Commit();
			OnCommited(command, item);
		}

		#endregion

		partial void OnDeleteUser(DeleteUserCommand command, ref User item);

		partial void OnDeletedUser(DeleteUserCommand command, User item);

		partial void OnCommit(DeleteUserCommand command, User item);

		partial void OnCommited(DeleteUserCommand command, User item);
	}
}

﻿#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Cqrs.MongoDB.Entities
{
	[Serializable]
	[DataContract]
	public abstract class MongoEntity : Cqrs.Entities.Entity
	{
		[Required]
		[BsonId]
		[DataMember]
		public override Guid Rsn { get; set; }

		[DataMember]
		public override int SortingOrder { get; set; }

		[Required]
		[DataMember]
		public override bool IsLogicallyDeleted { get; set; }
	}
}
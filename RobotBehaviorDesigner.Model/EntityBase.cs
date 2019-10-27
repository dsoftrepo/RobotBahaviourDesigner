using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace RobotBehaviourDesigner.Model
{
	public abstract class EntityBase
	{
		protected EntityBase()
		{
			this.Id = ObjectId.GenerateNewId().ToString();
		}

		[JsonIgnore]
		[BsonId]       
		[BsonIgnoreIfDefault]
		[BsonRepresentation(BsonType.ObjectId)]
		[Key]
		public string Id { get; set; }
		public DateTime CreatedOn { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string ModifiedBy { get; set; }
	}
}
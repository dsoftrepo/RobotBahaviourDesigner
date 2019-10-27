using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;

namespace RobotBehaviourDesigner.Model
{
	[TableName("Motions")]
	public class Motion : EntityBase
	{
		public Motion()
		{
			ServoMoves = new Collection<ServoMoveSequence>();
		}

		[Required] [StringLength(50)] public string Name { get; set; }
		public Collection<ServoMoveSequence> ServoMoves { get; set; }
	}
}
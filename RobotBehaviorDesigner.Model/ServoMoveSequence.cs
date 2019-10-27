using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace RobotBehaviourDesigner.Model
{
	public class ServoMoveSequence
	{
		[Required] public int ServoId { get; set; }
		[Required][StringLength(50)] public string ServoName { get; set; }
		[Required] public Collection<ServoPoint> ServoPoints { get; set; }
	}
}
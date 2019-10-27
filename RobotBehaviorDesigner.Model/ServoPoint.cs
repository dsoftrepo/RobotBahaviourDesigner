using System.ComponentModel.DataAnnotations;

namespace RobotBehaviourDesigner.Model
{
	public class ServoPoint
	{
		public ServoPoint(int index, double position)
		{
			Index = index;
			Position = position;
		}
		
		[Required] public int Index { get; set; }
		[Required] public double Position { get; set; }
	}
}

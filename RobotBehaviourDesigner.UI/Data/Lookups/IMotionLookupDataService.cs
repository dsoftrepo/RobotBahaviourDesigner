using System.Collections.Generic;
using System.Threading.Tasks;
using RobotBehaviourDesigner.Model;

namespace RobotBehaviourDesigner.UI.Data.Lookups
{
	public interface IMotionLookupDataService
	{
		Task<IEnumerable<LookupItem>> GetMotionLookupAsync();
	}
}
using RobotBehaviourDesigner.DataAccess.Database;
using RobotBehaviourDesigner.Model;

namespace RobotBehaviourDesigner.DataAccess.Repositories
{
	public class MotionRepository : Repository<Motion>
	{
		public MotionRepository(IDatabase database) : base(database)
		{
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RobotBehaviourDesigner.DataAccess.Repositories;
using RobotBehaviourDesigner.Model;

namespace RobotBehaviourDesigner.UI.Data.Lookups
{
	public class LookupDataService : IMotionLookupDataService
	{
		private readonly IRepository<Motion> _repository;

		public LookupDataService(IRepository<Motion> repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<LookupItem>> GetMotionLookupAsync()
		{
			IEnumerable<Motion> results = await _repository.GetAllAsync();
			return results?.Select(m =>
				new LookupItem
				{
					Id = m.Id.ToString(),
					DisplayMember = m.Name
				})
			.ToList();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RobotBehaviourDesigner.DataAccess.Database
{
	public interface IDatabase : IDisposable
	{
		Task<bool> Exists<T>(string filter);
		Task<bool> Exists<T>(Expression<Func<T, bool>> expression);
		Task<object> Read<T>(string filter = null);
		Task<object> Read<T>(Expression<Func<T, bool>> expression);
		Task<object> ReadSingle<T>(string filter);
		Task<object> ReadSingle<T>(Expression<Func<T, bool>> expression);
		Task<string[]> Create<T>(IEnumerable<T> entities);
		Task<string[]> Update<T>(IEnumerable<T> entities);
		Task<string[]> Replace<T>(IEnumerable<T> entities);
		Task<string[]> Delete<T>(IEnumerable<T> entities);
	}
}
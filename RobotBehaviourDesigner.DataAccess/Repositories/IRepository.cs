using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RobotBehaviourDesigner.DataAccess.Repositories
{
	public interface IRepository<T> : IDisposable
	{
		bool Exists(Expression<Func<T, bool>> predicate);
		Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
		T Find(Expression<Func<T, bool>> predicate);
		Task<T> FindAsync(Expression<Func<T, bool>> predicate);
		IEnumerable<T> Select(Expression<Func<T, bool>> predicate, bool cleanSearch = false);
		Task<IEnumerable<T>> SelectAsync(Expression<Func<T, bool>> predicate, bool cleanSearch = false);
		IEnumerable<T> GetAll();
		Task<IEnumerable<T>> GetAllAsync();
		void Add(T entity);
		void Update(T entity);
		void Replace(T entity);
		void Remove(T entity);
		bool HasChanges();
		Task<bool> SaveChangesAsync();
	}
}
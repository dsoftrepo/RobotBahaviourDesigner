using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RobotBehaviourDesigner.DataAccess.Database;
using RobotBehaviourDesigner.Model;

namespace RobotBehaviourDesigner.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        private readonly List<T> _entitiesToAdd;
        private readonly List<T> _entitiesToDelete;
        private readonly Dictionary<string, T> _entitiesToUpdate;
        private readonly Dictionary<string, T> _entitiesToReplace;
        private readonly ChangeTracker<T> _changeTracker;
        private readonly IDatabase _database;

		public Repository(IDatabase database)
        {
            _entitiesToAdd = new List<T>();
            _entitiesToDelete = new List<T>();
            _entitiesToUpdate = new Dictionary<string, T>();
            _entitiesToReplace = new Dictionary<string, T>();
			_changeTracker = new ChangeTracker<T>();
            _database = database;
        }

        public IEnumerable<T> GetAll()
        {
            return GetAllAsync().Result;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
	        var results = (IEnumerable<T>) await _database.Read<T>().ConfigureAwait(false);
	        IEnumerable<T> entityBases = results.ToList();
	        _changeTracker.StartTracking(entityBases);
			return entityBases;
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _database.Exists(predicate).Result;
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return _database.Exists(predicate);
        }

        public T Find(Expression<Func<T, bool>> expression)
        {
            return FindAsync(expression).Result;;
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> expression)
        {
	        var result = (T)await _database.ReadSingle(expression).ConfigureAwait(false);
			_changeTracker.StartTracking(result);
	        return result;
        }

        public IEnumerable<T> Select(Expression<Func<T, bool>> expression, bool cleanSearch = false)
        {
            return SelectAsync(expression, cleanSearch).Result;
        }

        public async Task<IEnumerable<T>> SelectAsync(Expression<Func<T, bool>> expression, bool cleanSearch = false)
        {
            var results =  (IEnumerable<T>) await _database.Read(expression).ConfigureAwait(false);
            IEnumerable<T> entityBases = results.ToList();
            _changeTracker.StartTracking(entityBases);
            return entityBases;
        }

        public void Add(T entity)
        {
			_changeTracker.StartTracking(entity);
            _entitiesToAdd.Add(entity);
        }

        public void Update(T entity)
        {
	        if (!_changeTracker.HasChanged(entity)) return;
            string keyValue = entity.Id;

            if (_entitiesToUpdate.ContainsKey(keyValue))
                _entitiesToUpdate[keyValue] = entity;
            else
                _entitiesToUpdate.Add(keyValue, entity);
        }

        public void Replace(T entity)
        {
	        if (!_changeTracker.HasChanged(entity)) return;
            string keyValue = entity.Id;

            if (_entitiesToReplace.ContainsKey(keyValue))
                _entitiesToReplace[keyValue] = entity;
            else
                _entitiesToReplace.Add(keyValue, entity);
        }

        public void Remove(T entity)
        {
            if (_entitiesToDelete.Any(x => Equals(x.Id, entity.Id))) return;
            _entitiesToDelete.Add(entity);
        }

        public bool HasChanges()
        {
	        return _entitiesToAdd.Any() || _entitiesToUpdate.Any() || _entitiesToReplace.Any() || _entitiesToDelete.Any();
        }

        public bool HasChanged(T entity)
        {
	        return _changeTracker.HasChanged(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (_entitiesToAdd.Any())
            {
                string[] results = await _database.Create(_entitiesToAdd).ConfigureAwait(false);
                _changeTracker.StartTracking(_entitiesToAdd);
                _entitiesToAdd.Clear();
            }

            if (_entitiesToUpdate.Any())
            {
                List<T> latestChanges = _entitiesToUpdate.Values.ToList();
                string[] results = await _database.Update(latestChanges).ConfigureAwait(false);
                _changeTracker.OnDbSaved(_entitiesToUpdate.Values);
                _entitiesToUpdate.Clear();
            }

            if (_entitiesToReplace.Any())
            {
                List<T> latestChanges = _entitiesToReplace.Values.ToList();
                string[] results = await _database.Replace(latestChanges).ConfigureAwait(false);
                _entitiesToReplace.Clear();
				_changeTracker.OnDbSaved(_entitiesToReplace.Values);
				_entitiesToReplace.Clear();
            }

            if (_entitiesToDelete.Any())
            {
                string[] deleteResults = await _database.Delete(_entitiesToDelete).ConfigureAwait(false);
                _changeTracker.StartTracking(_entitiesToDelete);
                _entitiesToDelete.Clear();
            }

            return true;
        }

        public void Dispose()
        {
            SaveChangesAsync().Wait();
            _changeTracker.StopTracking();
        }
    }
}
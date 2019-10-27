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
        private readonly List<T> _entitiesAdd;
        private readonly List<T> _entitiesDelete;
        private readonly Dictionary<string, T> _entitiesUpdate;
        private readonly Dictionary<string, T> _entitiesReplace;
        private readonly IDatabase _database;

        public Repository(IDatabase database)
        {
            _entitiesAdd = new List<T>();
            _entitiesDelete = new List<T>();
            _entitiesUpdate = new Dictionary<string, T>();
            _entitiesReplace = new Dictionary<string, T>();
            _database = database;
        }

        public IEnumerable<T> GetAll()
        {
            return GetAllAsync().Result;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
	        return (IEnumerable<T>) await _database.Read<T>().ConfigureAwait(false);
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
            return FindAsync(expression).Result;
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> expression)
        {
	        return (T)await _database.ReadSingle(expression).ConfigureAwait(false);
        }

        public IEnumerable<T> Select(Expression<Func<T, bool>> expression, bool cleanSearch = false)
        {
            return SelectAsync(expression, cleanSearch).Result;
        }

        public async Task<IEnumerable<T>> SelectAsync(Expression<Func<T, bool>> expression, bool cleanSearch = false)
        {
            return (IEnumerable<T>) await _database.Read(expression).ConfigureAwait(false);
        }

        public void Add(T entity)
        {
            _entitiesAdd.Add(entity);
        }

        public void Update(T entity)
        {
            string keyValue = entity.Id;

            if (_entitiesUpdate.ContainsKey(keyValue))
                _entitiesUpdate[keyValue] = entity;
            else
                _entitiesUpdate.Add(keyValue, entity);
        }

        public void Replace(T entity)
        {
            string keyValue = entity.Id;

            if (_entitiesReplace.ContainsKey(keyValue))
                _entitiesReplace[keyValue] = entity;
            else
                _entitiesReplace.Add(keyValue, entity);
        }

        public void Remove(T entity)
        {
            if (_entitiesDelete.Any(x => Equals(x.Id, entity.Id))) return;
            _entitiesDelete.Add(entity);
        }

        public bool HasChanges()
        {
	        return _entitiesAdd.Any() || _entitiesUpdate.Any() || _entitiesReplace.Any() || _entitiesDelete.Any();
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (_entitiesAdd.Any())
            {
                string[] results = await _database.Create(_entitiesAdd).ConfigureAwait(false);
                _entitiesAdd.Clear();
            }

            if (_entitiesUpdate.Any())
            {
                List<T> latestChanges = _entitiesUpdate.Values.ToList();
                string[] results = await _database.Update(latestChanges).ConfigureAwait(false);
                _entitiesUpdate.Clear();
            }

            if (_entitiesReplace.Any())
            {
                List<T> latestChanges = _entitiesReplace.Values.ToList();
                string[] results = await _database.Replace(latestChanges).ConfigureAwait(false);
                _entitiesReplace.Clear();
            }

            if (_entitiesDelete.Any())
            {
                string[] deleteResults = await _database.Delete(_entitiesDelete).ConfigureAwait(false);
                _entitiesDelete.Clear();
            }

            return true;
        }

        public void Dispose()
        {
            SaveChangesAsync().Wait();
        }
    }
}
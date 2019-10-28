using System.Collections.Generic;
using Newtonsoft.Json;
using RobotBehaviourDesigner.Model;

namespace RobotBehaviourDesigner.DataAccess.Repositories
{
	public class ChangeTracker<T>
	{
		private readonly Dictionary<object, string> _dbSavedEntities;

		public ChangeTracker()
		{
			_dbSavedEntities = new Dictionary<object, string>();
		}

		public bool HasChanged(T entity)
		{
			object key = entity.GetPrimaryKeyValue();
			return _dbSavedEntities.ContainsKey(key) && _dbSavedEntities[key] != JsonConvert.SerializeObject(entity);
		}

		public void OnDbSaved(IEnumerable<T> entities)
		{
			foreach (T entity in entities)
			{
				OnDbSaved(entity);
			}
		}

		public void OnDbSaved(T entity)
		{
			object key = entity.GetPrimaryKeyValue();
			if (_dbSavedEntities.ContainsKey(key))
			{
				_dbSavedEntities[key] = JsonConvert.SerializeObject(entity);
			}
		}

		public void StopTracking()
		{
			_dbSavedEntities.Clear();
		}

		public void StopTracking(T entity)
		{
			object key = entity.GetPrimaryKeyValue();
			if (_dbSavedEntities.ContainsKey(key))
			{
				_dbSavedEntities.Remove(key);
			}
		}

		public void StartTracking(T entity)
		{
			object key = entity.GetPrimaryKeyValue();
			if (!_dbSavedEntities.ContainsKey(key))
			{
				_dbSavedEntities.Add(key, JsonConvert.SerializeObject(entity));
			}
		}

		public void StartTracking(IEnumerable<T> entities)
		{
			foreach (T entity in entities)
			{
				StartTracking(entity);
			}
		}
	}
}

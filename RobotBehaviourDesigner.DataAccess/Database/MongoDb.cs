using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RobotBehaviourDesigner.Model;

namespace RobotBehaviourDesigner.DataAccess.Database
{
    public class MongoDataBase : IDatabase
    {
        private readonly IMongoDatabase _database;

        public MongoDataBase(string connectionString)
        {
	        var client = new MongoClient();
	        _database = client.GetDatabase(MongoUrl.Create(connectionString).DatabaseName);
        }

        public void Dispose()
        {
        }

        public async Task<bool> Exists<T>(string filter)
        {
	        IEnumerable<T> records;

	        if (string.IsNullOrEmpty(filter))
	        { 
		        records = await _database.GetCollection<T>(GetName<T>()).Find(new BsonDocument()).ToListAsync().ConfigureAwait(false);
	        }
	        else
	        { 
		        records = await _database.GetCollection<T>(GetName<T>()).Find(filter).ToListAsync().ConfigureAwait(false);    
	        }

	        return records.Any();
        }

        public async Task<bool> Exists<T>(Expression<Func<T, bool>> expression)
        {
	        List<T> records = await _database
		        .GetCollection<T>(GetName<T>())
		        .Find(expression)
		        .ToListAsync()
		        .ConfigureAwait(false);

	        return records.Any();
        }

        public async Task<object> Read<T>(Expression<Func<T, bool>> expression)
        {
            return await _database
                .GetCollection<T>(GetName<T>())
                .Find(expression)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<object> Read<T>(string filter = null)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await _database.GetCollection<T>(GetName<T>()).Find(new BsonDocument()).ToListAsync().ConfigureAwait(false);
            }

            return await _database.GetCollection<T>(GetName<T>()).Find(filter).ToListAsync().ConfigureAwait(false);
        }

        public async Task<object> ReadSingle<T>(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                await _database.GetCollection<T>(GetName<T>()).Find(new BsonDocument()).FirstOrDefaultAsync().ConfigureAwait(false);
            }

            return _database.GetCollection<T>(GetName<T>()).Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<object> ReadSingle<T>(Expression<Func<T, bool>> expression)
        {
            return await _database.GetCollection<T>(GetName<T>()).Find(expression).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<string[]> Create<T>(IEnumerable<T> entities)
        {
            IEnumerable<T> list = entities.ToList();
            try
            {
                await _database.GetCollection<T>(GetName<T>()).InsertManyAsync(list).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new[] { "error : " + ex.Message, ex.InnerException?.Message };
            }

            return list.Select(x => x.GetPrimaryKeyValue().ToString()).ToArray();
        }

        public async Task<string[]> Update<T>(IEnumerable<T> entities)
        {
            IEnumerable<T> list = entities.ToList();
            var updateTasks = new List<Task<T>>();
            try
            {
                IMongoCollection<T> collection = _database.GetCollection<T>(GetName<T>());
                foreach (T entity in list)
                {
                    FilterDefinition<T> filter = Builders<T>.Filter.Eq(entity.GetPrimaryKeyName(), entity.GetPrimaryKeyValue());
                    updateTasks.Add(collection.FindOneAndUpdateAsync(filter, entity.ToBsonDocument()));
                }

                T[] results = await Task.WhenAll(updateTasks).ConfigureAwait(false);
                return results.Select(x => x.GetPrimaryKeyValue().ToString()).ToArray();
            }
            catch (Exception ex)
            {
                return new[] { "error : " + ex.Message, ex.InnerException?.Message };
            }
        }

        public async Task<string[]> Replace<T>(IEnumerable<T> entities)
        {
            IEnumerable<T> list = entities.ToList();
            var replaceTasks = new List<Task<T>>();
            try
            {
                IMongoCollection<T> collection = _database.GetCollection<T>(GetName<T>());
                foreach (T entity in list)
                {
                    FilterDefinition<T> filter = Builders<T>.Filter.Eq(entity.GetPrimaryKeyName(), entity.GetPrimaryKeyValue());
                    replaceTasks.Add(collection.FindOneAndReplaceAsync(filter, entity));
                }

                T[] results = await Task.WhenAll(replaceTasks).ConfigureAwait(false);
                return results.Select(x => x.GetPrimaryKeyValue().ToString()).ToArray();
            }
            catch (Exception ex)
            {
                return new[] { "error : " + ex.Message, ex.InnerException?.Message };
            }
        }

        public async Task<string[]> Delete<T>(IEnumerable<T> entities)
        {
            var deleteTasks = new List<Task<DeleteResult>>();
            IEnumerable<T> list = entities.ToList();
            try
            {
                IMongoCollection<T> collection = _database.GetCollection<T>(GetName<T>());
                foreach (T entity in list)
                {
                    FilterDefinition<T> filter = Builders<T>.Filter.Eq(entity.GetPrimaryKeyName(), entity.GetPrimaryKeyValue());
                    deleteTasks.Add(collection.DeleteOneAsync(filter));
                }

                DeleteResult[] results = await Task.WhenAll(deleteTasks).ConfigureAwait(false);
                return results.Where(x => x.IsAcknowledged).Select(x => x.IsAcknowledged.ToString()).ToArray();
            }
            catch (Exception ex)
            {
                return new[] { "error : " + ex.Message, ex.InnerException?.Message };
            }
        }

        private static string GetName<T>()
        {
            return EntityObjectExtensions.GetTableName<T>();
        }
    }
}
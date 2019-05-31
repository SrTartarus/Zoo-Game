using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine;

namespace Zoo.Web
{
    public class Mongo
    {

        private MongoClient client;
        private IMongoDatabase db;

        public Mongo(string URI, string database)
        {
            client = new MongoClient(URI);
            db = client.GetDatabase(database);
        }

        /// <summary>
        /// Insert one element
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="collection">Collection to insert object</param>
        /// <param name="document">Object to insert</param>
        public void Insert<T>(string collection, T document)
        {
            // Insert
            db.GetCollection<T>(collection).InsertOne(document);
        }

        /// <summary>
        /// Insert one element async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="document"></param>
        /// <param name="callback"></param>
        public async void Insert<T>(string collection, T document, Action<bool> callback)
        {
            try
            {
                // Insert
                await db.GetCollection<T>(collection).InsertOneAsync(document);
                callback(true);
            }
            catch (MongoException e)
            {
                callback(false);
            }
        }

        public bool IsEmailExisted<T>(string email)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("email", email);
            return (db.GetCollection<T>("rpg_users").Find<T>(filter).ToList().Count > 0) ? true : false;
        }

        public bool IsUsernameExisted<T>(string username)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("username", username);
            return (db.GetCollection<T>("rpg_users").Find<T>(filter).ToList().Count > 0) ? true : false;
        }

        /// <summary>
        /// Select by FilterDefinition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T Select<T>(string collection, FilterDefinition<T> filter)
        {
            return db.GetCollection<T>(collection).Find(filter).First();
        }

        /// <summary>
        /// Select by field and value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Select<T>(string collection, string field, string value)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(field, value);
            return db.GetCollection<T>(collection).Find(filter).First();
        }

        /// <summary>
        /// Select by FilterDefinition async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <param name="action"></param>
        public async void Select<T>(string collection, FilterDefinition<T> filter, Action<bool, T> callback)
        {
            try
            {
                IMongoCollection<T> mongoCollection = db.GetCollection<T>(collection);
                List<T> newList = new List<T>();
                using (IAsyncCursor<T> cursor = await mongoCollection.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        IEnumerable<T> batch = cursor.Current;
                        foreach (T document in batch)
                        {
                            newList.Add(document);
                        }
                    }
                }

                if (newList.Count <= 0)
                    callback(false, default(T));
                else
                    callback(true, newList[0]);
            }
            catch (MongoException e)
            {
                callback(false, default(T));
            }

        }

        /// <summary>
        /// Select All elements by collection, NOTE:This function use async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="callback"></param>
        public async void SelectAll<T>(string collection, Action<bool, List<T>> callback)
        {
            try
            {
                IMongoCollection<T> mongoCollection = db.GetCollection<T>(collection);
                FilterDefinition<T> filter = FilterDefinition<T>.Empty;
                List<T> newList = new List<T>();
                using (IAsyncCursor<T> cursor = await mongoCollection.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        IEnumerable<T> batch = cursor.Current;
                        foreach (T document in batch)
                        {
                            newList.Add(document);
                        }
                    }
                }

                callback(true, newList);
            }
            catch (MongoException e)
            {
                callback(false, new List<T>());
            }
        }

        public async void SelectAll<T>(string collection, FilterDefinition<T> filter, Action<bool, List<T>> callback)
        {
            try
            {
                IMongoCollection<T> mongoCollection = db.GetCollection<T>(collection);
                List<T> newList = new List<T>();
                using (IAsyncCursor<T> cursor = await mongoCollection.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        IEnumerable<T> batch = cursor.Current;
                        foreach (T document in batch)
                        {
                            newList.Add(document);
                        }
                    }
                }

                callback(true, newList);
            }
            catch (MongoException e)
            {
                callback(false, new List<T>());
            }
        }

        /// <summary>
        /// Update one element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <param name="update"></param>
        public void Update<T>(string collection, ObjectId id, UpdateDefinition<T> update)
        {
            // Update
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", id);
            db.GetCollection<T>(collection).UpdateOne(filter, update);
        }

        /// <summary>
        /// Update one element async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <param name="callback"></param>
        public async void AsyncUpdate<T>(string collection, ObjectId id, UpdateDefinition<T> update, Action<bool> callback)
        {
            // Update
            try
            {
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", id);
                await db.GetCollection<T>(collection).UpdateOneAsync(filter, update);
                callback(true);
            }
            catch(MongoException e)
            {
                callback(false);
            }
            
        }

        /// <summary>
        /// Delete one element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        public void Delete<T>(string collection, ObjectId id)
        {
            // Delete
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", id);
            db.GetCollection<T>(collection).DeleteOne(filter);
        }

        /// <summary>
        /// Delete one element async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        public async void AsyncDelete<T>(string collection, ObjectId id, Action<bool> callback)
        {
            // Delete
            try
            {
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", id);
                await db.GetCollection<T>(collection).DeleteOneAsync(filter);
                callback(true);
            }
            catch(MongoException e)
            {
                callback(false);
            }
        }

        /// <summary>
        /// Dispose Mongo Client
        /// </summary>
        public void Shutdown()
        {
            if (client != null)
                client.Cluster.Dispose();

            client = null;
            db = null;
        }
    }

}

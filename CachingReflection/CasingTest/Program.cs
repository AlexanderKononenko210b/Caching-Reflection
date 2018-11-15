using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cashing;
using Microsoft.Win32;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CasingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = new User
            {
                FirstName = "Ivan",
                SecondName = "Petrov",
                Age = 35,
                AdditionInfo = new AdditionInfo
                {
                    Phone = "+375-(29)-756-45-67",
                    Fax = "+375-(17)-254-67-87"
                }
            };

            var key = "user";

            DataWrite<User>("user", user, TimeSpan.FromSeconds(5));

            if (KeyExist(key))
            {
                Console.WriteLine("The lifetime of the key has not yet expired.");
                var userDeserialize = DataRead<User>("user");
                Console.WriteLine("User data from cash:");
                Console.WriteLine(userDeserialize.ToString());
            }

            Thread.Sleep(6000);

            if (!KeyExist(key))
            {
                Console.WriteLine("The lifetime of the key has already expired. User data do not exist in database!");
            }
        }

        /// <summary>
        /// Write data to the Redis using key and 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">data key</param>
        /// <param name="data">data value</param>
        /// <param name="timeLifeKey">time life key</param>
        private static void DataWrite<T>(string key, T data, TimeSpan timeLifeKey)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                IDatabase database = redis.GetDatabase();

                var convertSettings = new JsonSerializerSettings { ContractResolver = new ContractResolver() };

                var jsonData = JsonConvert.SerializeObject(data, convertSettings);

                database.StringSet(key, jsonData);

                database.KeyExpire(key, TimeSpan.FromSeconds(5));

                redis.Close();
            }
        }

        /// <summary>
        /// Read data from Redis using key
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="key">key for data</param>
        /// <returns>instance type T or default(T)</returns>
        private static T DataRead<T>(string key)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                try
                {
                    IDatabase database = redis.GetDatabase();
                    var data = database.StringGet(key);
                    redis.Close();

                    if (data.IsNull)
                        return default(T);

                    return JsonConvert.DeserializeObject<T>(data);
                }
                catch
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Check exist key in database
        /// </summary>
        /// <param name="key">data key</param>
        /// <returns>if key exist in database true or false</returns>
        private static bool KeyExist(string key)
        {
            using (var redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                IDatabase database = redis.GetDatabase();

                if (!database.KeyExists(key))
                    return false;
                return true;
            }
        }
    }
}

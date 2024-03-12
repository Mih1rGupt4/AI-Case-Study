using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using AIRecommender.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.Diagnostics;

namespace AIRecommendation.DataService
{
    public class RedisDataCacher : IDataCacher
    {
        private readonly string _wslCommand;

        public RedisDataCacher()
        {
            _wslCommand = "redis-cli";
        }

        private string key = "bookdetails";

        static Logger dataLoadLogger = LogManager.GetLogger("DataLoadLogger");

        public BookDetails GetData()
        {
            string value = ExecuteRedisCliCommand($"GET {key}");
            if (string.IsNullOrWhiteSpace(value) || value.Contains("nil")) // Redis returns "nil" if the key does not exist
            {
                dataLoadLogger.Info("Data not found in cache.");
                return null;
            }

            dataLoadLogger.Info("\n\n--------\n\nData found in cache file. Returning cached data.\n\n--------\n\n");
            return JsonConvert.DeserializeObject<BookDetails>(value);
        }

        public void SetData(BookDetails bookDetails)
        {
            var expiry = TimeSpan.FromMinutes(5);
            var serializedValue = JsonConvert.SerializeObject(bookDetails);
            ExecuteRedisCliCommand($"SET {key} '{serializedValue}' EX {expiry.TotalSeconds}");
            dataLoadLogger.Info("\n--------\nMemory Caching Successfull\n--------\n");
        }

        private string ExecuteRedisCliCommand(string command)
        {
            var processInfo = new ProcessStartInfo()
            {
                Verb = "runas",
                LoadUserProfile = true,
                FileName = "cmd.exe",
                Arguments = $"/c wsl redis-cli {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception($"Error executing Redis CLI command: {error}");
                    }

                    return output.Trim();
                }
                else
                {
                    throw new Exception("Failed to start WSL process.");
                }
            }
        }
    }
    //public class RedisDataCacher : IDataCacher
    //{
    //    private readonly ConnectionMultiplexer _redisConnection;
    //    private string key = "book-details";

    //    static Logger dataLoadLogger = LogManager.GetLogger("DataLoadLogger");
    //    public RedisDataCacher(string connectionString)
    //    {
    //        _redisConnection = ConnectionMultiplexer.Connect(connectionString);
    //    }

    //    public BookDetails GetData()
    //    {
    //        var db = _redisConnection.GetDatabase();
    //        var value = db.StringGet(key);

    //        if (value.HasValue)
    //        {
    //            dataLoadLogger.Info("\n\n--------\n\nData found in cache file. Returning cached data.\n\n--------\n\n");
    //            return JsonConvert.DeserializeObject<BookDetails>(value);
    //        }
    //        else
    //        {
    //            dataLoadLogger.Info("Data not found in cache.");
    //            return null;
    //        }
    //    }

    //    public void SetData(BookDetails bookDetails)
    //    {
    //        var expiry = TimeSpan.FromMinutes(5);

    //        var db = _redisConnection.GetDatabase();
    //        var serializedValue = JsonConvert.SerializeObject(bookDetails);
    //        db.StringSet(key, serializedValue, expiry);
    //        dataLoadLogger.Info("\n--------\nMemory Caching Successfull\n--------\n");
    //    }
    //}
}

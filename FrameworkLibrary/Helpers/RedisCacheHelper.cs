﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace FrameworkLibrary
{
    public class RedisCacheHelper
    {
        private static string _connectionString = "";
        public static void SetRedisCacheConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(_connectionString);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static Return SaveToCache(string url, string html)
        {
            try
            {
                url = url.ToLower();
                IDatabase cache = Connection.GetDatabase();
                var returnVal = cache.StringSet(url, html, TimeSpan.FromSeconds(SettingsMapper.GetSettings().OutputCacheDurationInSeconds));

                return new Return();
            }
            catch (Exception ex)
            {
                ErrorHelper.LogException(ex);
                return new Return(ex, ErrorHelper.CreateError(ex));
            }
        }

        public static string GetFromCache(string url)
        {
            try
            {
                url = url.ToLower();
                IDatabase cache = Connection.GetDatabase();
                return cache.StringGet(url);
            }
            catch (Exception ex)
            {
                ErrorHelper.LogException(ex);
                return "";
            }
        }

        public static Return ClearCache(string url)
        {
            try
            {
                url = url.ToLower();
                IDatabase cache = Connection.GetDatabase();

                if (cache.KeyExists(url))
                {
                    cache.KeyDelete(url);
                }

                return new Return();
            }
            catch (Exception ex)
            {
                ErrorHelper.LogException(ex);
                return new Return(ex, ErrorHelper.CreateError(ex));
            }
        }
    }
}

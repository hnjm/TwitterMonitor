﻿using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Tweetinvi.Models;
using Wikiled.Common.Arguments;

namespace Wikiled.Twitter.Security
{
    public class PersistedAuthentication : IAuthentication
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IAuthentication underlying;

        public PersistedAuthentication(IAuthentication underlying)
        {
            Guard.NotNull(() => underlying, underlying);
            this.underlying = underlying;
        }

        public ITwitterCredentials Authenticate()
        {
            var file = "key.auth";
            string json;
            if (File.Exists(file))
            {
                log.Info("Found saved applicationCredentials. Loading...");
                json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<TwitterCredentials>(json);
            }

            var credentials = underlying.Authenticate();
            json = JsonConvert.SerializeObject((TwitterCredentials)credentials);
            string jsonFormatted = JToken.Parse(json).ToString(Formatting.Indented);
            File.WriteAllText(file, jsonFormatted);
            return credentials;
        }
    }
}

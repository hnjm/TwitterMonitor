﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Tweetinvi;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Streaming;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Twitter.Persistency;
using Wikiled.Twitter.Security;

namespace Wikiled.Twitter.Streams
{
    public class MonitoringStream : IDisposable
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private IFilteredStream stream;

        private readonly IPersistency persistency;

        private int isActive;

        private long totalReceived;

        private readonly IAuthentication auth;

        private readonly HashSet<long> following = new HashSet<long>();

        public MonitoringStream(IPersistency persistency, IAuthentication auth)
        {
            Guard.NotNull(() => persistency, persistency);
            Guard.NotNull(() => auth, auth);
            this.persistency = persistency;
            this.auth = auth;
        }

        public bool IsActive
        {
            get => Interlocked.CompareExchange(ref isActive, 0, 0) == 1;
            private set => Interlocked.Exchange(ref isActive, value ? 1 : 0);
        }

        public long TotalReceived => Interlocked.Read(ref totalReceived);

        public async void Start(string[] keywords, string[] follows)
        {
            Guard.NotNull(() => keywords, keywords);
            log.Debug("Starting...");
            IsActive = true;

            Auth.InitializeApplicationOnlyCredentials(Credentials.Instance.IphoneTwitterCredentials);
            ExceptionHandler.SwallowWebExceptions = false;
            ExceptionHandler.WebExceptionReceived += ExceptionHandlerOnWebExceptionReceived;

            stream = Stream.CreateFilteredStream(auth.Authenticate());
            stream.JsonObjectReceived += StreamOnJsonObjectReceived;
            foreach (var keyword in keywords)
            {
                log.Debug("Add track {0}", keyword);
                stream.AddTrack(keyword);
            }

            if (follows != null)
            {
                foreach (var follow in follows)
                {
                    IUser user = User.GetUserFromScreenName(follow);
                    following.Add(user.Id);
                    log.Debug("Add follow {0}", user);
                    stream.AddFollow(user);
                }
            }

            stream.StallWarnings = true;
            stream.LimitReached += StreamOnLimitReached;
            stream.StreamStarted += StreamOnStreamStarted;
            stream.StreamStopped += StreamOnStreamStopped;
            stream.WarningFallingBehindDetected += StreamOnWarningFallingBehindDetected;
            do
            {
                try
                {
                    await stream.StartStreamMatchingAnyConditionAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                if (IsActive)
                {
                    log.Info("Waiting to retry");
                    await Task.Delay(500).ConfigureAwait(false);
                }
            }
            while (IsActive);
        }

        private void StreamOnJsonObjectReceived(object sender, JsonObjectEventArgs jsonObjectEventArgs)
        {
            try
            {
                Interlocked.Increment(ref totalReceived);
                var json = jsonObjectEventArgs.Json;
                var jsonConvert = TweetinviContainer.Resolve<IJsonObjectConverter>();
                var tweetDto = jsonConvert.DeserializeObject<ITweetDTO>(json);
                Task.Run(() => persistency.Save(tweetDto));
                if (tweetDto.CreatedBy != null)
                {
                    log.Debug("Message received: [{0}-{3}] - [{1}-{2}]", tweetDto.CreatedBy.Location, tweetDto.CreatedBy.Name, tweetDto.CreatedBy.FollowersCount, tweetDto.Place);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ExceptionHandlerOnWebExceptionReceived(object sender, GenericEventArgs<ITwitterException> genericEventArgs)
        {
            log.Error(genericEventArgs.Value.WebException);
        }

        public void Stop()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            stream?.StopStream();
        }

        private void StreamOnWarningFallingBehindDetected(object sender, WarningFallingBehindEventArgs warningFallingBehindEventArgs)
        {
            string message = $"Falling behind {warningFallingBehindEventArgs.WarningMessage}...";
            log.Warn(message);
        }

        private void StreamOnStreamStopped(object sender, StreamExceptionEventArgs streamExceptionEventArgs)
        {
            log.Warn("Stream Stopped...");
        }

        private void StreamOnStreamStarted(object sender, EventArgs eventArgs)
        {
            log.Warn("Stream started...");
        }

        private void StreamOnLimitReached(object sender, LimitReachedEventArgs limitReachedEventArgs)
        {
            string message = $"Limit reatched: {limitReachedEventArgs.NumberOfTweetsNotReceived}";
            log.Info(message);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
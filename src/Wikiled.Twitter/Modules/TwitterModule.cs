﻿using Autofac;
using Tweetinvi;
using Wikiled.Twitter.Communication;
using Wikiled.Twitter.Discovery;
using Wikiled.Twitter.Persistency;
using Wikiled.Twitter.Streams;

namespace Wikiled.Twitter.Modules
{
    public class TwitterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            builder.RegisterType<MessageDiscovery>().As<IMessageDiscovery>();
            builder.RegisterType<MonitoringStream>().As<IMonitoringStream>();
            builder.RegisterType<MessagesDownloader>().As<IMessagesDownloader>();
            builder.RegisterType<FileLoader>().As<IFileLoader>();
            builder.RegisterType<Publisher>().As<IPublisher>();
            builder.RegisterType<UserStream>().As<IUserStream>();
            base.Load(builder);
        }
    }
}

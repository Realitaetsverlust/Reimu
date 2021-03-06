﻿using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using Reimu.Database;
using Reimu.Common.Data;
using VndbSharp;

namespace Reimu.Core
{
    public class BotContext : ShardedCommandContext
    {
        public IDocumentSession Session { get; }
        public DatabaseHandler Database { get; }
        public BotConfig Config { get; }
        public GuildConfig GuildConfig { get; }
        public UserData UserData { get; }
        public Vndb VndbClient { get; }

        public BotContext(DiscordShardedClient client, SocketUserMessage msg, IServiceProvider provider) : base(client,
            msg)
        {
            Database = provider.GetRequiredService<DatabaseHandler>();
            Session = Database.GetSession();
            Config = Database.Get<BotConfig>("Config");
            VndbClient = provider.GetRequiredService<Vndb>();
            UserData = Database.Get<UserData>($"user-{User.Id}") ?? new UserData {Id = $"user-{User.Id}"};

            // NOTE: In DMs `Guild` is null which will cause this to throw, effectively killing most DM capabilities.
            // If we want DM commands we need to do additional checks here, DiscordHandler, etc however I do not see
            // enough of a benefit currently to write extra checks.
            GuildConfig = Database.Get<GuildConfig>($"guild-{Guild.Id}");
        }
    }
}

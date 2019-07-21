using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordMinesweeper
{

    class Program
    {
        private readonly DiscordSocketClient _client;
        string mention;
        readonly config myConf;

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        static void Main(string[] args)
        {
            string arg;

            foreach (string s in args)
            {
                if (s == "--help" || s == "-h")
                {
                    Console.WriteLine("Usage: DiscordMinesweeper [config]");
                    Console.WriteLine("If not provided, the bot will search for \"config.toml\"");
                    Console.WriteLine("The config file is a TOML file, with 1 namespace (\"config\"), containing 2 values, \"token\" and \"command\"");
                    return;
                }
            }
            arg = (args.Length == 0 ? "config.toml" : args[0]);

            new Program(arg).MainAsync().GetAwaiter().GetResult();
        }

        public Program(string fname)
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            myConf = ConfigGetter.GetConfigFromTOML(fname);
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task MainAsync()
        {
            //Console.WriteLine(string.Format("Token is {0}"), myConf.token);
            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, myConf.token);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            if (myConf.command == "@")
            {
                try
                {
                    mention = "<@" + _client.CurrentUser.Id.ToString() + ">";
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error: Could not get the user id");
                    throw e;
                }
            }
            else
            {
                mention = myConf.command;
            }
            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == mention)
                await message.Channel.SendMessageAsync(MapGenerator.GenerateMap(9, 9, 10).ToDiscordEmoji());

        }
    }
}

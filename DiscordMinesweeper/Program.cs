using Discord;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordMinesweeper
{

    class Program
    {
        private readonly DiscordSocketClient _client;
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
            _client.SlashCommandExecuted += SlashCommandHandler;
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
        private async Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            var existing_commands = await _client.GetGlobalApplicationCommandsAsync();

            if (!existing_commands.Any(x => x.Name == "minesweeper")) {
                Console.WriteLine("slashCommand is not registered ! Registering now");
                var minesweeperCommand = new SlashCommandBuilder();
                minesweeperCommand.WithName("minesweeper");
                minesweeperCommand.WithDescription("Generate a minesweeper field in this channel !");
                try
                {
                    // With global commands we don't need the guild.
                    await _client.CreateGlobalApplicationCommandAsync(minesweeperCommand.Build());
                    // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                    // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
                }
                catch (ApplicationCommandException)
                {
                    Console.Error.WriteLine("Error: Could not register the slash command");

                    // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                    throw;
                }
            } else
            {
                Console.WriteLine("slashCommand is already registered, skipping");
            }
            return;
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            if (command.Data.Name != "minesweeper") return;
            await command.RespondAsync(MapGenerator.GenerateMap(9, 9, 10).ToDiscordEmoji());
        }
    }
}

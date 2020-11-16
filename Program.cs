using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;
using Discord_Bot.Commands;

internal class Program
{
    private CancellationTokenSource _cts { get; set; }
    private IConfigurationRoot _config;

    private DiscordClient _discord;
    private CommandsNextModule _commands;
    private InteractivityModule _interactivity;

    static async Task Main(string[] args) => await new Program().InitBot(args);

    async Task InitBot(string[] args)
    {
        try
        {
            Console.WriteLine("[info] Welcome to my bot!");
            _cts = new CancellationTokenSource();

            Console.WriteLine("[info] Loading config file..");
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            Console.WriteLine("[info] Creating discord client..");
            _discord = new DiscordClient(new DiscordConfiguration
            {
                Token = _config.GetValue<string>("discord:token"),
                TokenType = TokenType.Bot
            });

            _interactivity = _discord.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehaviour = TimeoutBehaviour.Delete,
                PaginationTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(30)
            });


            var deps = BuildDeps();
            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = _config.GetValue<string>("discord:CommandPrefix"),
                Dependencies = deps
            });

            Console.WriteLine("[info] Loading command modules..");

            var type = typeof(IModule);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

            var typeList = types as Type[] ?? types.ToArray();
            foreach (var t in typeList)
                _commands.RegisterCommands(t);

            Console.WriteLine($"[info] Loaded {typeList.Count()} modules.");

            RunAsync(args).Wait();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
        }
    }

    async Task RunAsync(string[] args)
    {
        Console.WriteLine("Connecting..");
        await _discord.ConnectAsync();
        Console.WriteLine("Connected!");

        while (!_cts.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromMinutes(1));
    }
    private DependencyCollection BuildDeps()
    {
        using var deps = new DependencyCollectionBuilder();

        deps.AddInstance(_interactivity)
            .AddInstance(_cts)
            .AddInstance(_config)
            .AddInstance(_discord);
        return deps.Build();
    }
}
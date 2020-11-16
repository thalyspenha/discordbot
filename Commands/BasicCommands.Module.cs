using Discord_Bot.Commands;
using Discord_Bot.Helpers;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;

public class BasicCommandsModule : IModule
{
    private Files files;
    private DiscordEmbedBuilder embed;

    [Command("alive")]
    [Description("Simple command to test if the bot is running!")]
    public async Task Alive(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();
        await ctx.RespondAsync("I'm alive!");
    }

    [Command("interact")]
    [Description("Simple command to test interaction!")]
    public async Task Interact(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();
        await ctx.RespondAsync("How are you today?");

        var intr = ctx.Client.GetInteractivityModule();
        var reminderContent = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id,
            TimeSpan.FromSeconds(60)
        );

        if (reminderContent == null)
        {
            await ctx.RespondAsync("Sorry, I didn't get a response!");
            return;
        }

        await ctx.RespondAsync("Thank you for telling me how you are!");
    }

    [Command("sendnudes")]
    public async Task SendImages(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();
        files = new Files();
        var image = files.GetRandomImage();
        await ctx.RespondWithFileAsync(image);
        
    }

    [Command("userinfo")]
    public async Task BotInfo(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();
       
        string botAvatar = ctx.User.AvatarUrl;
        DateTimeOffset date = ctx.User.CreationTimestamp;
        string username = ctx.User.Username;
                
        embed = new DiscordEmbedBuilder();

        embed.Color = DiscordColor.Azure;
        embed.ThumbnailUrl = botAvatar;
        embed.WithAuthor("🤖 Minhas informações");
        embed.AddField("Nick", username);
        embed.AddField("Id", ctx.User.Id.ToString());
        embed.AddField("Inscrito em ",date.ToString("dd/MM/yyyy H:mm:ss "));
        embed.WithFooter("2020 Porner");

        
        await ctx.Channel.SendMessageAsync("", false, embed);


      


    }

}
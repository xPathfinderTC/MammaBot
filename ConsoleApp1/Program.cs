using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Main
{
    class Program
    {
        public static Program Instance { get { return instance; } }
        private static Program instance = new Program();

        private CommandService commandService;
        private IServiceProvider services;

        // NET 0.9 이상의 버전에서는 더이상 DiscordClient를 사용하지 않는다.
        // 따라서 DiscordSocketClient 클래스를 사용한다.
        private DiscordSocketClient discordSocketClient;

        private Dictionary<string, string> commands;

        private static string STATIC_TOKEN = "NDgyODQ5MzQ3MjE4ODMzNDE4.DmK5IA.tgrn7hUIXrIYcSI2eaAlCzOJZWo";

        public async Task GetHandler(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            int argPos = 0;

            if (true == message.HasStringPrefix("!헬프", ref argPos))
            {
                await HelpMessageSender(socketMessage);
            }
            else if (0 == message.Content.CompareTo(NTable.Food.Keyword))
            {
                string reply = NTable.Food.Instance.GetRandomObject();
                await MessageSender(socketMessage, reply);
            }
            else
            {
                await SimpleMessageSender(socketMessage);
            }
        }

        public async Task MessageSender(SocketMessage socketMessage, string reply)
        {
            if (null == socketMessage)
            {
                return;
            }

            var message = socketMessage as SocketUserMessage;

            var command = new CommandContext(discordSocketClient, message);
            var result = await command.Channel.SendMessageAsync(reply);
            return;
        }

        public async Task SimpleMessageSender(SocketMessage socketMessage)
        {
            if (null == socketMessage)
            {
                return;
            }

            var message = socketMessage as SocketUserMessage;
            int argPos = 0;
            string reply = string.Empty;

            int maxCount = 0;
            foreach (var item in commands)
            {
                if (true == message.HasStringPrefix(item.Value, ref argPos))
                {
                    maxCount++;
                }
            }
            DateTime dateTime = DateTime.Now;

            Random rnd = new Random(dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Year);
            int time = rnd.Next(1, maxCount);

            int index = 1;
            foreach (var item in commands)
            {
                if (true == message.HasStringPrefix(item.Value, ref argPos))
                {
                    if (index == time || 1 == maxCount)
                    {
                        reply = item.Key;
                        break;
                    }
                    index++;
                }
            }

            var command = new CommandContext(discordSocketClient, message);
            var result = await command.Channel.SendMessageAsync(reply);
            return;
        }

        public async Task HelpMessageSender(SocketMessage socketMessage)
        {
            if (null == socketMessage)
            {
                return;
            }

            var message = socketMessage as SocketUserMessage;
            string reply = string.Empty;

            if (message.Content.CompareTo("!헬프") != 0)
            {
                return;
            }
            else
            {
                reply = "Developed by xPathfinder with Csharp Language\n";
                reply += "현재 메뉴 추천봇은 다음과 같은 명령어를 가지고 있습니다.\n";

                string text = string.Format("- {0} : 반응 {1} 개 보유 중, Flyweight Datasize : {2}bytes", NTable.Food.Keyword, NTable.Food.Flyweight.Count.ToString(), NTable.Food.Instance.GetSize().ToString());
                reply += text + "\n";

                text = string.Format("- {0} : 반응 {1} 개 보유 중, Flyweight Datasize : {2}bytes", NTable.Mamma.Keyword, NTable.Mamma.Flyweight.Count.ToString(), NTable.Mamma.Instance.GetSize().ToString());
                reply += text + "\n";

                var command = new CommandContext(discordSocketClient, message);
                var result = await command.Channel.SendMessageAsync(reply);
                return;
            }
        }

        public async Task BuildCommands()
        {
            discordSocketClient.MessageReceived += GetHandler;

            await commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public void GenerateSimpleCommand(string type, string response)
        {
            if (true == this.commands.Keys.Contains(type))
            {
                Console.WriteLine("ERROR : " + type + " 값 타입이 이미 존재합니다.");
                return;
            }
            this.commands.Add(type, response);
        }

        public void TableBuild(string fileName, string type)
        {
            if (true == this.commands.Keys.Contains(type))
            {
                Console.WriteLine("ERROR : " + type + " 값 타입이 이미 존재합니다.");
                return;
            }

            if ( type.CompareTo(NTable.Food.Keyword) == 0 )
            {
                NTable.Food.Instance.Execute(fileName);
            }
            else if (type.CompareTo(NTable.Mamma.Keyword) == 0)
            {
                NTable.Mamma.Instance.Execute(fileName);
            }
        }

        private Task Log(LogMessage e)
        {
            if ( null != e.Exception && e.Exception.GetType() == typeof(ArgumentOutOfRangeException) )
            {
                return Task.CompletedTask;
            }
            Console.WriteLine(e.Message);
            return Task.CompletedTask;
        }

        public async Task MainAsync()
        {

            this.commandService = new CommandService();

            this.services = new ServiceCollection().BuildServiceProvider();
            this.commands = new Dictionary<string, string>();
            this.discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            if (null == this.discordSocketClient)
            {
                await Task.Delay(-1);
            }

            TableBuild(@"C:\Users\xstel\source\repos\ConsoleApp1\ConsoleApp1\Table\Food.csv", NTable.Food.Keyword);
            TableBuild(@"C:\Users\xstel\source\repos\ConsoleApp1\ConsoleApp1\Table\Mamma.csv", NTable.Mamma.Keyword);
            GenerateSimpleCommand("@TREEMAN 효도 좀 하자", "선우야");

            this.discordSocketClient.Log += Log;

            await BuildCommands();

            await this.discordSocketClient.LoginAsync(TokenType.Bot, STATIC_TOKEN);
            await this.discordSocketClient.StartAsync();

            await Task.Delay(-1);
        }

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

    }
}

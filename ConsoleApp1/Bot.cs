using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NObject.NBot
{
    public class Bot
    {

        private CommandService commandService;
        private IServiceProvider services;

        // NET 0.9 이상의 버전에서는 더이상 DiscordClient를 사용하지 않는다.
        // 따라서 DiscordSocketClient 클래스를 사용한다.
        private DiscordSocketClient discordSocketClient;

        private Dictionary<string, string> commands;

        private static string STATIC_TOKEN = "NDgyODQ5MzQ3MjE4ODMzNDE4.DmK5IA.tgrn7hUIXrIYcSI2eaAlCzOJZWo";
        private List<NTable.Flyweight> flyweights = new List<NTable.Flyweight>();

        public async Task GetHandler(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;

            if (0 == message.Content.CompareTo("!헬프"))
            {
                await HelpMessageSender(socketMessage);
            }
            else
            {
                NTable.Flyweight flyweight = null;
                foreach( var item in flyweights )
                {
                    if ( item.GetFlyweightType().CompareTo(message.Content) == 0 )
                    {
                        flyweight = item;
                        break;
                    }
                }

                if (null != flyweight)
                {
                    string reply = flyweight.GetItem();
                    await MessageSender(socketMessage, reply);
                }
                else
                {
                    await SimpleMessageSender(socketMessage);
                }
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

                foreach (var item in flyweights)
                {
                    string text = string.Format("- {0}\nResponse Count : {1}\nFlyweight Datasize : {2}bytes\nDatapath : {3}", item.GetFlyweightType(), item.GetCount().ToString(), item.GetSize().ToString(), item.GetFilePath());
                    reply += text + "\n";
                }

                reply += string.Format("* 현재 서버에서의 메모리 사용량은 작업관리자 기준 {0}입니다.", GetMemoryUsage());

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

        private Task Log(LogMessage e)
        {
            if (null != e.Exception && e.Exception.GetType() == typeof(ArgumentOutOfRangeException))
            {
                return Task.CompletedTask;
            }
            Console.WriteLine(e.Message);
            return Task.CompletedTask;
        }

        public bool AddFlyweight(NTable.Flyweight flyweight)
        {
            bool isSuccess = true;

            foreach (var item in flyweights)
            {
                if ( item.GetFlyweightType().CompareTo(flyweight.GetFlyweightType()) == 0 )
                {
                    isSuccess = false;
                }
            }

            flyweights.Add(flyweight);

            return isSuccess;
        }

        private string GetMemoryUsage()
        {
            try
            {
                string fname = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

                ProcessStartInfo ps = new ProcessStartInfo("tasklist");
                ps.Arguments = "/fi \"IMAGENAME eq " + fname + ".*\" /FO CSV /NH";
                ps.RedirectStandardOutput = true;
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                var p = Process.Start(ps);
                if (p.WaitForExit(1000))
                {
                    var s = p.StandardOutput.ReadToEnd().Split('\"');
                    return s[9].Replace("\"", "");
                }
            }
            catch { }
            return "Unable to get memory usage";
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

            // 여기다 테이블 인스턴스를 추가하세요.
            // 자동으로 테이블을 읽어오고, 명령어로 등록됩니다.
            bool isSuccess = false;
            isSuccess = AddFlyweight(NTable.Food.Instance);
            isSuccess = AddFlyweight(NTable.Mamma.Instance);
            isSuccess = AddFlyweight(NTable.EnglishWord.Instance);

            // 중복되는 명령어가 있으면 Assert 됩니다.
            System.Diagnostics.Debug.Assert(isSuccess);

            // 테이블을 빌드합니다.
            foreach (var flyweight in flyweights)
            {
                if (null != flyweight)
                {
                    flyweight.TableBuild();
                }
            }

            // 테스트.
            GenerateSimpleCommand("효도 좀 하자", "선우야");

            this.discordSocketClient.Log += Log;

            await BuildCommands();

            await this.discordSocketClient.LoginAsync(TokenType.Bot, STATIC_TOKEN);
            await this.discordSocketClient.StartAsync();

            await Task.Delay(-1);
        }


        // 테스트용입니다.
        public void GenerateSimpleCommand(string type, string response)
        {
            if (true == this.commands.Keys.Contains(type))
            {
                Console.WriteLine("ERROR : " + type + " 값 타입이 이미 존재합니다.");
                return;
            }
            this.commands.Add(type, response);
        }

        // 테스트용입니다.
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
    }
}

using System;

namespace NObject.NMain
{
    class Program
    {
        static void Main(string[] args)
        {
            NObject.NBot.Bot Bot = new NObject.NBot.Bot();
            Bot.MainAsync().GetAwaiter().GetResult();
        }
    }
}

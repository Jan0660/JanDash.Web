using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JanDash
{
    public static class Program
    {
        public static string Version { get; private set; }
        public static Config Config { get; private set; }

        public static async Task Main(string[] args)
        {
            Version = typeof(Program).Assembly.GetName().Version!.ToString();
            Config = JsonSerializer.Deserialize<Config>(File.ReadAllText("./config.json"));
            Console.WriteLine("Connecting to MongoDb...");
            await Mongo.Connect();
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });


        public static void LimitedAdd<T>(this List<T> list, T item, int maxCount)
        {
            list.Add(item);
            if (list.Count == maxCount)
                list.Remove(list.FirstOrDefault());
        }
    }
}
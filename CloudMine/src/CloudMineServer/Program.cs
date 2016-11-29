using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace CloudMineServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();

            ////Set connection
            //var connection = new HubConnection("http://127.0.0.1:8088/");
            ////Make proxy to hub based on hub name on server
            //var myHub = connection.CreateHubProxy("CloudHub");
        }
    }
}

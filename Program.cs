using ConsoleApp1;
using EELVL;
using Microsoft.Extensions.Configuration;
using PixelPilot;
using PixelPilot.PixelGameClient;
using PixelPilot.PixelGameClient.Messages.Received;
using PixelPilot.PixelGameClient.Messages.Send;
// Load the configuration. Don't store your account token in the code :)
var configuration = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .AddEnvironmentVariables()
    .Build();

LogManager.Configure(configuration.GetSection("Logging"));
var config = configuration.Get<BasicConfig>();
if (config == null)
{
    Console.WriteLine("The configuration file could not be loaded.");
    return;
}
// Create a client.
var client = new PixelPilotClient(config.AccountToken, false);

// Executed once the client receives INIT
// Make a platform and do some silly loops.
client.OnClientConnected +=  (_) =>
{
    new System.Threading.Thread(() => BlockUploader.PlaceBlocks(client)).Start();

};
await client.Connect("4bcnhr8y8qcvecl");

Thread.Sleep(-1);
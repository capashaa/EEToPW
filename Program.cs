using ConsoleApp1;
using EELVL;
using Microsoft.Extensions.Configuration;
using PixelPilot;
using PixelPilot.PixelGameClient;
using PixelPilot.PixelGameClient.Messages.Received;
using PixelPilot.PixelGameClient.Messages.Send;
PixelPilotClient client_;
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
if (config.AccountEmail != null && config.AccountPassword != null)
{
    client_ = new PixelPilotClient(config.AccountEmail, config.AccountPassword, false);
}
else
{
    client_ = new PixelPilotClient(config.AccountToken, false);
}

// Executed once the client receives INIT
// Make a platform and do some silly loops.
client_.OnClientConnected += (_) =>
{
    new System.Threading.Thread(() => BlockUploader.PlaceBlocks(client_)).Start();

};

//Replace worldID with your OWNED WORLD worldid
//Example link in Webbrowser: https://pixelwalker.net/world/4bcnhr8y8qcvecl
//Will be: 4bcnhr8y8qcvecl
string worldID = "4bcnhr8y8qcvecl";
await client_.Connect(worldID);

Thread.Sleep(-1);
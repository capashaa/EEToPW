﻿using ConsoleApp1;
using EELVL;
using Microsoft.Extensions.Configuration;
using PixelPilot;
using PixelPilot.PixelGameClient;
using PixelPilot.PixelGameClient.Messages.Received;
using PixelPilot.PixelGameClient.Messages.Send;
using System.Runtime.CompilerServices;
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


await client_.Connect(SettingsFile.settings.WorldID);

Thread.Sleep(-1);
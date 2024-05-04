## EEToPW
Everybody Edits levels to PixelWalker  

# How to use this tool:
Add config.json into the bin folder:  
Don't remember to add your authtoken or email and password. 
**PLEASE DON'T SHARE THIS FILE WITH OTHERS**  
```{
  "AccountToken": "Your Authtoken goes here",
  "LoginEmail": "Email",
  "LoginPassword": "Password",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "PixelPilot.API": "Information",
      "PixelPilot.Client": "Information",
      "PixelPilot.World": "Information",
      "PixelPilot.PacketConverter": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```
Go to Program.cs, add your WorldID there and run.  

## Misc
[Using Marten's PixelWalker SDK](https://github.com/MartenM/PixelPilot)  
[Using EELVL SDK by Luke](https://gitlab.com/LukeM212/EELVL)  

Everything else is hardcoded. But meh.  

## Not supported yet:
Rotateable blocks (Not spikes)  
Effects

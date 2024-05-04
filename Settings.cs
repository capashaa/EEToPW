namespace ConsoleApp1
{


    public static class SettingsFile
    {

        public static Settings settings = new Settings()
        {
            //Add your eelvl filename inside here. The file need to be inside BIN.
            EELVLFileName = "fun.eelvl",

            //Replace worldID with your OWNED WORLD worldid
            //Example link in Webbrowser: https://pixelwalker.net/world/4bcnhr8y8qcvecl
            //Will be: 4bcnhr8y8qcvecl
            WorldID = "4bcnhr8y8qcvecl"
        };
    }

    public class Settings
    {
        public string EELVLFileName { get; set; } = null!;
        public string WorldID { get; set; } = null!;
    }
}


using Nett;
using System.Collections.Generic;

namespace DiscordMinesweeper
{
    class config
    {
        public string token { get; set; }
        public string command { get; set; }
        public config(string token, string command)
        {
            this.token = token;
            this.command = command;
        }
    }
    static class ConfigGetter
    {
        public static config GetConfigFromTOML(string file)
        {
            var tomlConf = Toml.ReadFile(file).ToDictionary();
            var myConf = (Dictionary<string, object>)tomlConf["config"];
            return new config(myConf["token"].ToString(), myConf["command"].ToString());
        }
    }
}

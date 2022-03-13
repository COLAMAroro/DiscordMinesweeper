using Nett;
using System.Collections.Generic;

namespace DiscordMinesweeper
{
    class config
    {
        public string token { get; set; }
        public config(string token)
        {
            this.token = token;
        }
    }
    static class ConfigGetter
    {
        public static config GetConfigFromTOML(string file)
        {
            var tomlConf = Toml.ReadFile(file).ToDictionary();
            var myConf = (Dictionary<string, object>)tomlConf["config"];
            return new config(myConf["token"].ToString());
        }
    }
}

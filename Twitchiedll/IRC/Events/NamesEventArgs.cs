using System.Collections.Generic;

namespace Twitchiedll.IRC.Events
{
    public class NamesEventArgs
    {
        public Dictionary<string, List<string>> Names = new Dictionary<string, List<string>>();

        public void GetNames(string buffer)
        {
            var parameters = buffer.Split(' ');

            if (parameters[4].StartsWith("#"))
            {
                var channel = parameters[4].TrimStart('#');

                if (!Names.ContainsKey(channel))
                    Names.Add(channel, new List<string>());

                if (parameters[5].StartsWith(":"))
                    Names[channel].AddRange(parameters[5].TrimStart(':').Split(' '));
            }
        }
    }
}
namespace Twitchiedll.IRC.Events
{
    public class HostTargetEventArgs
    {
        public int Viewers { get; internal set; }
        public string Channel { get; internal set; }
        public string TargetChannel { get; internal set; }
        public bool IsStarting { get; internal set; }

        public HostTargetEventArgs(string ircMessage)
        {
            var splittedMsg = ircMessage.Split(' ');

            int viewers;

            Viewers = int.TryParse(splittedMsg[4], out viewers) ? viewers : 0;
            Channel = splittedMsg[2].TrimStart('#');

            if (splittedMsg[3] != ":-")
            {
                TargetChannel = splittedMsg[3].Replace(":", "");
                IsStarting = true;
            }
            else
            {
                TargetChannel = "";
                IsStarting = false;
            }
        }
    }
}
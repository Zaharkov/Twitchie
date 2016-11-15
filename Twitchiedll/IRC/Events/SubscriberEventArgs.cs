﻿namespace Twitchiedll.IRC.Events
{
    public class SubscriberEventArgs
    {
        public string Username { get; set; }
        public string Channel { get; set; }
        public int Months { get; set; }
        public string Message { get; set; }

        public SubscriberEventArgs(string ircMessage)
        {
            var splittedMessage = ircMessage.Split(' ');
            int months;

            Username = splittedMessage[3].Remove(0, 1);
            Channel = splittedMessage[2].TrimStart('#');
            Message = ircMessage.Split(':')[2];

            if (splittedMessage[4].Equals("just") && splittedMessage[5].StartsWith("subscribed"))
                Months = 1;
            else if (splittedMessage[4].Equals("subscribed") && splittedMessage[6].Equals("for"))
                if (int.TryParse(splittedMessage[7], out months))
                    Months = months;
        }
    }
}
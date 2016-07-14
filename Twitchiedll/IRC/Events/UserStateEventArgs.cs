namespace Twitchiedll.IRC.Events
{
    public class UserStateEventArgs
    {
        public int UserId { get; internal set; }
        public string DisplayName { get; internal set; }
        public string ColorHex { get; internal set; }
        public UserType UserType { get; internal set; }
        public string Channel { get; internal set; }
        public string RawIrcMessage { get; internal set; }
        public string EmoteSet { get; internal set; }

        public UserStateEventArgs(string ircString)
        {
            RawIrcMessage = ircString;

            foreach (var split in ircString.Split(' '))
            {
                if (split.StartsWith("#"))
                {
                    Channel = split.TrimStart('#');
                    continue;
                }

                if (split.StartsWith("@"))
                {
                    foreach (var part in split.TrimStart('@').Split(';'))
                    {
                        if (part.Contains("color="))
                        {
                            if (ColorHex == null)
                                ColorHex = part.Split('=')[1];

                            continue;
                        }

                        if (part.Contains("display-name="))
                        {
                            if (DisplayName == null)
                                DisplayName = part.Split('=')[1];

                            continue;
                        }

                        if (part.Contains("emote-sets="))
                        {
                            if (EmoteSet == null)
                                EmoteSet = part.Split('=')[1];

                            continue;
                        }

                        if (part.Contains("subscriber="))
                        {
                            if(part.Split('=')[1] == "1")
                                UserType |= UserType.Subscriber;

                            continue;
                        }

                        if (part.Contains("broadcaster/1"))
                        {
                            UserType |= UserType.Broadcaster;
                            continue;
                        }

                        if (part.Contains("moderator/1"))
                        {
                            UserType |= UserType.Moderator;
                            continue;
                        }

                        if (part.Contains("turbo="))
                        {
                            if(part.Split('=')[1] == "1")
                                UserType |= UserType.Turbo;

                            continue;
                        }

                        if (part.Contains("user-id="))
                        {
                            UserId = int.Parse(part.Split('=')[1]);
                            continue;
                        }

                        if (part.Contains("user-type="))
                        {
                            switch (part.Split('=')[1].Split(' ')[0])
                            {
                                case "mod":
                                    UserType |= UserType.Moderator;
                                    break;
                                case "global_mod":
                                    UserType |= UserType.Globalmoderator;
                                    break;
                                case "admin":
                                    UserType |= UserType.Admin;
                                    break;
                                case "staff":
                                    UserType |= UserType.Staff;
                                    break;
                                default:
                                    UserType |= UserType.Viewer;
                                    break;
                            }
                            continue;
                        }

                        if (part.Contains("mod="))
                        {
                            if(part.Split('=')[1] == "1")
                                UserType |= UserType.Moderator;
                        }
                    }
                }
            }
        }
    }
}
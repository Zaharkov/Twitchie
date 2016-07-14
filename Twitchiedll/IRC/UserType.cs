using System;

namespace Twitchiedll.IRC
{
    [Flags]
    public enum UserType
    {
        Default = 1,
        Viewer = 2,
        Moderator = 4,
        Globalmoderator = 8,
        Admin = 16,
        Staff = 32,
        Subscriber = 64,
        Broadcaster = 128,
        Turbo = 256
    }
}

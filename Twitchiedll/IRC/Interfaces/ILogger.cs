using System;

namespace Twitchiedll.IRC.Interfaces
{
    public interface ILogger
    {
        void LogException(string message, Exception e);
    }
}

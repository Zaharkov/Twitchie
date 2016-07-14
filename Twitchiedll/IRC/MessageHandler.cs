using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Twitchiedll.IRC
{
    public class MessageHandler
    {
        protected TextWriter Writer;

        public MessageHandler(TextWriter writer)
        {
            Writer = writer;
        }

        public void WriteRawMessage(string rawMessage)
        {
            Writer.WriteLine(rawMessage);
            Writer.Flush();
        }

        public void WriteRawMessage(List<string> rawMessage)
        {
            var builder = new StringBuilder();

            foreach (var data in rawMessage)
                builder.AppendLine(data);

            WriteRawMessage(builder.ToString());
        }

        public void SendMessage(MessageType messageType, string channel, string message)
        {
            switch (messageType)
            {
                case MessageType.Action:
                    WriteRawMessage($"PRIVMSG #{channel} :/me {message}");
                    break;

                case MessageType.Message:
                    WriteRawMessage($"PRIVMSG #{channel} :{message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Twitchiedll.IRC
{
    public class MessageHandler : IDisposable
    {
        /*
        It's a global limit and the 100 threshold only applies 
        if you are only sending messages or commands in channels you are a mod in.
        For example: You send 50 messages to a channel where you are modded 
        and then send 1 message to a channel where you are not modded within 30 seconds. 
        Since 50 + 1 = 51 and 51 > 20, you will have exceeded the limit for non-modded channels
        and be locked out globally (by IP) for 2 hours.

        Whispers don't impact your channel send limit
        */
        public const int SimpleUserLimit = 20;
        public const int ModeratorUserLimit = 100;
        public static int GlobalMessageLimit = SimpleUserLimit;

        public readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

        private static readonly List<MessageContainer> MessageContainers = new List<MessageContainer>();
        private static readonly object LockForCheck = new object();
        private static readonly object LockForSend = new object();
        private readonly TextWriter _textWriter;

        public MessageHandler(TextWriter writer)
        {
            _textWriter = writer;
        }

        public void WriteRawMessage(string rawMessage, bool whisper = false, bool needWait = false)
        {
            var container = new MessageContainer(rawMessage);

            if (whisper)
            {
                WriteMessage(container);
                return;
            }

            Action action = () =>
            {
                var added = false;
                while (!IsCanSendMessage(container, TokenSource.Token, needWait, ref added))
                    Thread.Sleep(1000);

                if (!needWait)
                    TokenSource.Token.ThrowIfCancellationRequested();

                WriteMessage(container);
            };

            if (needWait)
            {
                var task = Task.Run(action);
                task.Wait();
            }
            else
            {
                Task.Run(action, TokenSource.Token);
            }
        }

        public void SendMessage(MessageType messageType, string channel, string message)
        {
            var whisper = channel.Equals("jtv");

            switch (messageType)
            {
                case MessageType.Action:
                    WriteRawMessage($"PRIVMSG #{channel} :/me {message}", whisper);
                    break;

                case MessageType.Message:
                    WriteRawMessage($"PRIVMSG #{channel} :{message}", whisper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }

        private void WriteMessage(MessageContainer messageContainer)
        {
            lock (LockForSend)
            {
                messageContainer.Time = DateTime.Now;
                _textWriter.WriteLine(messageContainer.Message);
                _textWriter.Flush();
            }
        }

        private static bool IsCanSendMessage(MessageContainer messageContainer, CancellationToken token, bool needWait, ref bool added)
        {
            lock (LockForCheck)
            {
                if (!added)
                {
                    MessageContainers.Add(messageContainer);
                    added = true;
                }

                if (token.IsCancellationRequested && !needWait)
                {
                    MessageContainers.Remove(messageContainer);
                    token.ThrowIfCancellationRequested();
                }

                var timeNow = DateTime.Now.AddSeconds(-30);
                var count = 0;
                var copy = MessageContainers.ToList();

                foreach (var message in copy)
                {
                    if (message.Time > timeNow)
                        count++;
                    else
                        MessageContainers.Remove(message);
                }

                return count < GlobalMessageLimit;
            }
        }

        public void Dispose()
        {
            if(!TokenSource.IsCancellationRequested)
                TokenSource.Cancel();

            _textWriter.Dispose();
        }
    }
}
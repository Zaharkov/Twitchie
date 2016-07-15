using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Twitchiedll.IRC.Events;

namespace Twitchiedll.IRC
{
    public partial class Twitchie : IDisposable
    {
        private string _buffer;
        private readonly List<string> _channels = new List<string>();
        private readonly NamesEventArgs _namesEventArgs = new NamesEventArgs();

        private TextReader _textReader;
        private TextWriter _textWriter;
        private MessageHandler _messageHandler;
        private TcpClient _clientSocket;

        public void Connect(string server, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(server, port);

            if (!_clientSocket.Connected)
                throw new Exception("Connection failed");

            var stream = _clientSocket.GetStream();

            _textReader = new StreamReader(stream);
            _textWriter = new StreamWriter(stream);
            _messageHandler = new MessageHandler(_textWriter);
        }

        public virtual void Login(string nick, string password)
        {
            _messageHandler.WriteRawMessage(new List<string>
            {
                $"USER {nick}",
                $"PASS {password}",
                $"NICK {nick}"
            });

            _buffer = _textReader.ReadLine();

            if(_buffer == null)
                throw new Exception("Login failed");

            if (_buffer.Split(' ')[1] != "001")
                throw new Exception("Registration Failed. Welcome message expected");

            _messageHandler.WriteRawMessage(new List<string>
            {
                "CAP REQ :twitch.tv/membership",
                "CAP REQ :twitch.tv/commands",
                "CAP REQ :twitch.tv/tags"
            });
        }

        public void Listen()
        {
            while ((_buffer = _textReader.ReadLine()) != null)
            {
                if (_buffer != null)
                    HandleEvents();
            }
        }

        public void Pong(string ping)
        {
            _messageHandler.WriteRawMessage(ping.Replace("PING", "PONG"));
        }

        public void Join(string channel)
        {
            _channels.Add(channel);
            _messageHandler.WriteRawMessage($"JOIN #{channel}");
        }

        public void Part(string channel)
        {
            _messageHandler.WriteRawMessage($"PART #{channel}");
        }

        public void PartFromAll()
        {
            foreach (var channel in _channels)
                Part(channel);
        }

        public void Whisper(string user, string message)
        {
            Message("jtv", $"/w {user} {message}");
        }

        public void Message(string channel, string message)
        {
            _messageHandler.SendMessage(MessageType.Message, channel, message);
        }

        public virtual void Quit()
        {
            _messageHandler.WriteRawMessage("QUIT");
        }

        public void Dispose()
        {
            _textReader.Dispose();
            _textWriter.Dispose();
            _clientSocket.Dispose();
        }
    }
}
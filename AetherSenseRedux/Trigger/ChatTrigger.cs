using AetherSenseRedux.Pattern;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AetherSenseRedux.Trigger
{
    internal class ChatTrigger : ITrigger
    {
        // ITrigger properties
        public bool Enabled { get; set; }
        public string Type { get; } = "ChatTrigger";
        public string Name { get; init; }
        public List<Device> Devices { get; init; }
        public List<string> EnabledDevices { get; init; }
        public string Pattern { get; init; }
        public PatternConfig PatternSettings { get; init; }

        // ChatTrigger properties
        private List<ChatMessage> _messages;
        public Regex Regex { get; init; }
        public long RetriggerDelay { get; init; }
        private DateTime RetriggerTime { get; set; }

        public ChatTrigger(ChatTriggerConfig configuration, ref List<Device> devices)
        {
            // ITrigger properties
            Enabled = true;
            Name = configuration.Name;
            Devices = devices;
            EnabledDevices = configuration.EnabledDevices;
            Pattern = configuration.Pattern;
            PatternSettings = configuration.PatternSettings;
            Regex = new Regex(configuration.Regex);
            RetriggerDelay = configuration.RetriggerDelay;

            // ChatTrigger properties
            _messages = new List<ChatMessage>();
            RetriggerTime = DateTime.MinValue;

        }


        public void Queue(ChatMessage message)
        {
            if (Enabled)
            {
                _messages.Add(message);
            }
        }

        private void OnTrigger()
        {
            if (RetriggerDelay > 0)
            {
                if (DateTime.UtcNow < RetriggerTime)
                {
                    return;
                }
                else
                {
                    RetriggerTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(RetriggerDelay);
                }
            }
            foreach (Device device in Devices)
            {
                if (EnabledDevices.Contains(device.Name))
                {
                    device.Patterns.Add(PatternFactory.GetPatternFromString(Pattern, PatternSettings));
                }

            }
        }

        public void Start()
        {
            Task.Run(() => Run());
        }

        public void Stop()
        {
            Enabled = false;
        }

        public async Task Run()
        {
            while (Enabled)
            {
                if (_messages.Count > 0)
                {
                    foreach (ChatMessage message in _messages)
                    {
                        
                        if (Regex.IsMatch(message.ToString()))
                        {
                            OnTrigger();
                        }

                        _messages.Remove(message);
                        await Task.Yield();
                    }
                }
                await Task.Delay(10);
            }
        }
        static TriggerConfig GetDefaultConfiguration()
        {
            return new ChatTriggerConfig();
        }
    }

    [Serializable]
    public class ChatTriggerConfig : TriggerConfig
    {
        public override string Type { get; } = "Chat";
        public override string Name { get; set; } = "New Chat Trigger";
        public string Regex { get; set; } = "Your Regex Here";
        public long RetriggerDelay { get; set; } = 0;
    }

    struct ChatMessage
    {

        public ChatMessage(XivChatType chatType, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            ChatType = chatType;
            SenderId = senderId;
            Sender = sender;
            Message = message;
            IsHandled = isHandled;
        }

        public XivChatType ChatType;
        public uint SenderId;
        public SeString Sender;
        public SeString Message;
        public bool IsHandled;

        public override string ToString()
        {
            return string.Format("{2}: <{0}> {1}", Sender.TextValue, Message.TextValue, ChatType.ToString());
        }
    }
}

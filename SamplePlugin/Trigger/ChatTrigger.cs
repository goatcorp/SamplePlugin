using AetherSenseRedux.Pattern;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Trigger
{
    internal class ChatTrigger
    {
        // ITrigger properties
        public bool Enabled { get; set; }
        public string Name;
        private readonly List<Device> Devices;
        private readonly List<string> EnabledDevices;
        private readonly string Pattern;
        private readonly Dictionary<string, object> PatternSettings;

        // ChatTrigger properties
        private readonly List<ChatMessage> _messages;
        private readonly string _regex;
        private readonly long _retriggerDelay;
        private DateTime _retriggerTime;

        public ChatTrigger(string name, ref List<Device> devices, List<string> enabledDevices, string pattern, Dictionary<string,object> patternSettings, string regex, long retriggerDelay)
        {
            // ITrigger properties
            Enabled = true;
            Name = name;
            Devices = devices;
            EnabledDevices = enabledDevices;
            Pattern = pattern;
            PatternSettings = patternSettings;

            // ChatTrigger properties
            _messages = new List<ChatMessage>();
            _regex = regex;
            _retriggerDelay = retriggerDelay;
            _retriggerTime = DateTime.MinValue;

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
            if (_retriggerDelay > 0)
            {
                if (DateTime.UtcNow < _retriggerTime)
                {
                    return;
                }
                else
                {
                    _retriggerTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(_retriggerDelay);
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

        public async Task Run()
        {
            while (Enabled)
            {
                if (_messages.Count > 0)
                {
                    foreach (ChatMessage message in _messages)
                    {
                        //TODO: search for match and call OnTrigger() if matched
                        _messages.Remove(message);
                        await Task.Yield();
                    }
                }
                await Task.Delay(10);
            }
        }
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

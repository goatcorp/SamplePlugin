using AetherSenseRedux.Pattern;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dalamud.Logging;
using System.Collections.Concurrent;

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
        private ConcurrentQueue<ChatMessage> _messages;
        public Regex Regex { get; init; }
        public long RetriggerDelay { get; init; }
        private DateTime RetriggerTime { get; set; }
        private object queueLock = new object();
        private Guid Guid { get; set; }

        public ChatTrigger(ChatTriggerConfig configuration, ref List<Device> devices)
        {
            // ITrigger properties
            Enabled = true;
            Name = configuration.Name;
            Devices = devices;
            EnabledDevices = configuration.EnabledDevices;
            Pattern = configuration.Pattern;
            PatternSettings = PatternFactory.GetPatternConfigFromObject(configuration.PatternSettings);
            Regex = new Regex(configuration.Regex);
            RetriggerDelay = configuration.RetriggerDelay;

            // ChatTrigger properties
            _messages = new ConcurrentQueue<ChatMessage>();
            RetriggerTime = DateTime.MinValue;
            Guid = Guid.NewGuid();

        }


        public void Queue(ChatMessage message)
        {
            if (Enabled)
            {
                 PluginLog.Verbose("{0} ({1}): Received message to queue",Name, Guid.ToString());
                 _messages.Enqueue(message);
            }
        }

        private void OnTrigger()
        {
            if (RetriggerDelay > 0)
            {
                if (DateTime.UtcNow < RetriggerTime)
                {
                    PluginLog.Debug("Trigger discarded, too soon");
                    return;
                }
                else
                {
                    RetriggerTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(RetriggerDelay);
                }
            }
            lock (Devices)
            {
                foreach (Device device in Devices)
                {
                    if (EnabledDevices.Contains(device.Name))
                    {
                        lock (device.Patterns)
                        {
                            device.Patterns.Add(PatternFactory.GetPatternFromString(Pattern, PatternSettings));
                        }
                    }

                }
            }
        }

        public void Start()
        {
            Task.Run(MainLoop).ConfigureAwait(false); ;
        }

        public void Stop()
        {
            Enabled = false;
        }

        public async Task MainLoop()
        {
            while (Enabled)
            {
                ChatMessage message;
                if (_messages.TryDequeue(out message))
                {
                    PluginLog.Verbose("{1}: Processing message: {0}", message.ToString(), Guid.ToString());
                    if (Regex.IsMatch(message.ToString()))
                    {
                        OnTrigger();
                        PluginLog.Debug("{1}: Triggered on message: {0}", message.ToString(), Guid.ToString());
                    }
                    await Task.Yield();
                } else
                {
                    await Task.Delay(50);
                }
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

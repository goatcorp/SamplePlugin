using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace Dancingway
{
    public class Emotelist
    {
        private List<Emote> emoteList;
        private Random rnd;

        // Public external methods
        public void InitialiseEmoteList()
        {
            emoteList = new List<Emote>();
            rnd = new Random();
        }

        public void BuildEmoteList()
        {
            // do nothing for now...
        }

        public void TestBuild()
        {
            if (!emoteList.Any())   // if the list is empty
            {
                // do nothing for now...
            } else  // list is not empty
            {
                emoteList.Clear();
            }

            AddEmote("/hdance");
            AddEmote("/dance");
            AddEmote("/mdance");
            AddEmote("/bdance");
            AddEmote("/sdance");
            AddEmote("/mogdance");
            AddEmote("/sundance");
        }

        // Private internal methods
        private string Sanitise(string emoteToClean)
        {
            // doesnt do anything, yet
            string sanitised = emoteToClean;
            return sanitised;
        }

        // Public Getters
        public int getLength()
        {
            return emoteList.Count();
        }

        public string getRandomDance()
        {
            int index = rnd.Next(1, this.getLength());
            string chosenDance = getEmoteCommand(index);

            return chosenDance;
        }

        public string getEmoteCommand(int index)
        {
            string cleanEmote = Sanitise(emoteList[index].command);
            return cleanEmote;
        }

        // Public Setters
        public void AddEmote(string newCommand)
        {
            string cleanEmote = Sanitise(newCommand);
            string command = cleanEmote; // use the name and command as the same thing for now
            Emote newEmote;
            newEmote.command = cleanEmote;
            newEmote.name = cleanEmote;
            newEmote.enabled = true; // later on will add method to check if toon has this emote enabled

            emoteList.Add(newEmote);
        }



    }
}

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MyListLibrary;
using ListSymbolTable;
using TreeSymbolTable;

namespace Markov
{
    ///<summary>
    ///Holds all of the information for Markov Model needs for tracking single n-character substring.
    ///Contains string {state} - holds n-characters substring it represents.
    ///Contains int {count} - total number of occurences of this substring in source text.
    ///</summary>
    public class MarkovEntry
    {
        private string state;
        private int count;
        private MyList<char> suffixes;
        Random rnd;

        public MarkovEntry(string state)
        {
            this.state = state;
            count = 0;
            suffixes = new MyList<char>();
            rnd = new Random();
        }

        public void Add(char ch)
        {
            count++;
            suffixes.Add(ch);
        }

        public char RandomLetter()
        {
            return suffixes[rnd.Next(0, suffixes.Count)];
        }

        public override string ToString()
        {
            string result = $"MarkovEntry: \'{state}\' ({count}): ";
            int length = 5;
            if (count > 5)
            {
                result += $"{suffixes[0]} {suffixes[1]} {suffixes[2]} {suffixes[3]} {suffixes[4]} ... ({count-length}) more suffixes.";
            } else if (count == 5) {
                result += $"{suffixes[0]} {suffixes[1]} {suffixes[2]} {suffixes[3]} {suffixes[4]}";
            } else if (count == 0) {
                result += $"no suffixes.";
            } else {
                result += $"{suffixes[0]} ... ({count-length}) more suffixes.";
            }
            return result;
        }
    }

    ///<summary>
    ///Records information in a LinkedList Symbol Table.
    ///</summary>
    public class ListMarkovModel
    {
        string lines;
        int markovDegreeNumber;
        int storyLength;
        ListSymbolTable<string, MarkovEntry> database;

        public ListMarkovModel(string lines, int markovDegreeNumber, int storyLength)
        {
            this.lines = lines;
            this.markovDegreeNumber = markovDegreeNumber;
            this.storyLength = storyLength;
            database = new ListSymbolTable<string, MarkovEntry>();

            //creates database
            for (int i = 0; i < lines.Length - markovDegreeNumber; i++) //beginning; end - length of state; increment;
            {
                string state = lines.Substring(i, markovDegreeNumber);
                char next = lines[i+markovDegreeNumber];
                //construct MarkovModel
                if (!database.Contains(state))
                {
                    database.Add(state, new MarkovEntry(state));
                }
                database[state].Add(next);
            }
        }

        public string CreateStory()
        {
            string story = "";
            string state = "";
            char c;

            //first state
            state = lines.Substring(0, markovDegreeNumber);
            c = database[state].RandomLetter();
            story = state + c;
            state = IncrementState(state, c);

            for (int i = 0; i < storyLength; i++)
            {
                c = database[state].RandomLetter();
                story = story + c;
                state = IncrementState(state, c);
            }
            return story;
        }

        public string IncrementState(string state, char c)
        {
            state += c;
            state = state.Substring(1); //gets all char after the first to retain correct MarkovDegreeNumber length
            return state;
        }

        public override string ToString()
        {
            return database.Count + " nodes";
        }
    }
}

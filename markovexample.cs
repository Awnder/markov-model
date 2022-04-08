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
        int totalChar = 0; // tracks total characters when flow over storyLength
        string endings = ".?!";
        string capitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Random rnd = new Random();
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
            int count = 0;

            //first state
            state = RandomStart();
            c = database[state].RandomLetter();
            story = story + state + c;
            state = IncrementState(state, c);
            totalChar = totalChar + markovDegreeNumber + 1;

            while (!endings.Contains(c) || count < storyLength)
            {
                if (state.Length == markovDegreeNumber && database[state] != null)
                {
                    c = database[state].RandomLetter();
                    story = story + c;
                    state = IncrementState(state, c);
                    count++;
                    totalChar++;
                }
            }
            return story;
        }

        public string RandomStart()
        {
            int start = rnd.Next(0, lines.Length - markovDegreeNumber);
            string state = lines.Substring(start, markovDegreeNumber);
            while (state.Contains(' ') || !capitals.Contains(state.Substring(0,1)))
            {
                start = rnd.Next(0, lines.Length - markovDegreeNumber);
                state = lines.Substring(start, markovDegreeNumber);
            }
            return state;
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
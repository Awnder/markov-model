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
    ///</summary>
    public class MarkovEntry
    {
        private string state;
        private int count;
        private MyList<char> suffixes;
        Random rnd;

        ///<summary>
        ///Constructor takes string state and initializes private variables.
        ///</summary>
        public MarkovEntry(string state)
        {
            this.state = state;
            count = 0;
            suffixes = new MyList<char>();
            rnd = new Random();
        }

        ///<summary>
        ///Increases the amount of characters and adds the character to the LinkedList.
        ///</summary>
        public void Add(char ch)
        {
            count++;
            suffixes.Add(ch);
        }

        ///<summary>
        ///Gets a random number between 0 and the list's length, then uses that number to search through the list and 
        ///returns the character at the specified index.
        ///</summary>
        public char RandomLetter()
        {
            return suffixes[rnd.Next(0, suffixes.Count)];
        }

        ///<summary>
        ///Prints out the state, the amount of characters associated with that state, and some of the characters.
        ///</summary>
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
    ///Abstract class inherited by the other three models.
    ///Contains variables consistent that the other three need so less copy/pasting code
    ///</summary>
    public abstract class MarkovModel
    {
        protected string lines;
        protected int markovDegreeNumber;
        protected int storyLength;
        protected int totalChar = 0; //tracks total characters when flow over storyLength
        protected string endings = ".?!";
        protected string capitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        protected Random rnd = new Random();
        protected MyList<string> wordDictionary = new MyList<string>();

        ///<summary>
        ///Constructor takes the lines of text from file, the length of the state, and the length of the story
        ///</summary>
        public MarkovModel(string lines, int markovDegreeNumber, int storyLength)
        {
            this.lines = lines;
            this.markovDegreeNumber = markovDegreeNumber;
            this.storyLength = storyLength;
        }

        public abstract string CreateStory();
        public abstract string IncrementState(string state, char c);
        public abstract string RandomStart();
        // public abstract void CreateWordDictionary();
        // public abstract bool ValidWord(string state);

        public int TotalChar { get {return totalChar;} }
    }

    ///<summary>
    ///Records information in a LinkedList Symbol Table.
    ///</summary>
    public class ListMarkovModel : MarkovModel
    {
        ListSymbolTable<string, MarkovEntry> database;

        ///<summary>
        ///Constructor implements abstract class. see ": base (lines, markovDegreeNumber, storyLength)"
        ///Reads through file and stores every state and character associated with that state in a SymbolTable database
        ///</summary>
        public ListMarkovModel(string lines, int markovDegreeNumber, int storyLength) : base (lines, markovDegreeNumber, storyLength)
        {
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

            // CreateWordDictionary();
        }

        ///<summary>
        ///Records information in a LinkedList Symbol Table.
        ///</summary>
        public override string CreateStory()
        {
            string story = ""; //stores the story as a whole
            string state = ""; //stores each state that program iterates through
            char c;            //stores each character generated according to each state
            int count = 0;     //stores amount of characters generated

            //first state
            //state = lines.Substring(0, markovDegreeNumber);
            state = RandomStart();
            c = database[state].RandomLetter();
            story = story + state + c; //adds the beginning to the start of the story as well as the next character
            state = IncrementState(state, c);
            totalChar = totalChar + markovDegreeNumber + 1;

            //loops until the next character is ".?!" or the count < storyLength
            while (!endings.Contains(c) || count < storyLength)
            {
                //last check to see if the state is the right length and the state is actually in the database
                if (state.Length == markovDegreeNumber && database[state] != null)
                {
                    c = database[state].RandomLetter(); //gets a random letter
                    story = story + c; //adds the letter to the story
                    state = IncrementState(state, c); //moves the state to the next one forward
                    count++;
                    totalChar++;
                }
            }
            return story;
        }

        ///<summary>
        ///Finds a random state that starts with a capital letter and does not have a space in it.
        ///</summary>
        public override string RandomStart()
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

        ///<summary>
        ///Adds the next character to the state, then cuts of the first character of the state to retain correct state length.
        ///</summary>
        public override string IncrementState(string state, char c)
        {
            state += c;
            state = state.Substring(1); //gets all char after the first to retain correct MarkovDegreeNumber length
            return state;
        }

        // public override void CreateWordDictionary()
        // {
        //     foreach(string state in database)
        //     {
        //         string trimmed = String.Concat(state.Where(c => !Char.IsWhiteSpace(c)));
        //         if (trimmed.Length == markovDegreeNumber)
        //         {
        //             wordDictionary.Add(trimmed);
        //         }
        //     }
        // } 

        // public override bool ValidWord(string state)
        // {
        //     if (wordDictionary.Contains(state))
        //     {
        //         return true;
        //     } else {
        //         return false;
        //     }
        // }

        ///<summary>
        ///Returns amount of database nodes.
        ///</summary>
        public override string ToString()
        {
            return database.Count + " nodes";
        }
    }

    ///<summary>
    ///Records information in a Binary Tree Symbol Table.
    ///</summary>
    public class TreeMarkovModel : MarkovModel
    {
        TreeSymbolTable<string, MarkovEntry> database;

        public TreeMarkovModel(string lines, int markovDegreeNumber, int storyLength) : base (lines, markovDegreeNumber, storyLength)
        {
            database = new TreeSymbolTable<string, MarkovEntry>();

            //creates database
            for (int i = 0; i < lines.Length - markovDegreeNumber; i++) //beginning; end - length of state; increment;
            {
                string state = lines.Substring(i, markovDegreeNumber);
                char next = lines[i+markovDegreeNumber];
                //construct MarkovModel
                if (!database.ContainsKey(state))
                {
                    database.Add(state, new MarkovEntry(state));
                }
                database[state].Add(next);
            }
        }

        public override string CreateStory()
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
                if (state.Length == markovDegreeNumber && database[state] != null) //error checking just in case states not stored or retrieved correctly
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

        public override string RandomStart()
        {
            int start = rnd.Next(0, lines.Length - markovDegreeNumber);
            string state = lines.Substring(start, markovDegreeNumber);
            // finds state without spaces in it and with a beginning capital letter
            while (state.Contains(' ') || !capitals.Contains(state.Substring(0,1)))
            {
                start = rnd.Next(0, lines.Length - markovDegreeNumber);
                state = lines.Substring(start, markovDegreeNumber);
            }
            return state;
        }

        public override string IncrementState(string state, char c)
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

    ///<summary>
    ///Records information in .NET's Sorted Dictionary.
    ///</summary>
    public class DictMarkovModel : MarkovModel
    {
        SortedDictionary<string, MarkovEntry> database;

        public DictMarkovModel(string lines, int markovDegreeNumber, int storyLength) : base (lines, markovDegreeNumber, storyLength)
        {
            database = new SortedDictionary<string, MarkovEntry>();

            //creates database
            for (int i = 0; i < lines.Length - markovDegreeNumber; i++) //beginning; end - length of state; increment;
            {
                string state = lines.Substring(i, markovDegreeNumber);
                char next = lines[i+markovDegreeNumber];
                //construct MarkovModel
                if (!database.ContainsKey(state))
                {
                    database.Add(state, new MarkovEntry(state));
                }
                database[state].Add(next);
            }
        }

        public override string CreateStory()
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

        public override string RandomStart()
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

        public override string IncrementState(string state, char c)
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
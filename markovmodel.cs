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
        private MyList<char> suffixes = new MyList<char>();
        Random rnd = new Random();

        ///<summary>
        ///Constructor takes string state and initializes count to 0.
        ///</summary>
        public MarkovEntry(string state)
        {
            this.state = state;
            count = 0;
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
    ///Records information in a LinkedList Symbol Table.
    ///</summary>
    public class ListMarkovModel
    {
        string lines;       //lines from the text
        int stateLength;    //state length
        int storyLength;    //story length
        int totalChar = 0;  //tracks total characters
        Random rnd = new Random();
        ListSymbolTable<string, MarkovEntry> database = new ListSymbolTable<string, MarkovEntry>();

        ///<summary>
        ///Reads through file and stores every state and character associated with that state in a SymbolTable database
        ///</summary>
        public ListMarkovModel(string lines, int stateLength, int storyLength)
        {
            this.lines = lines;
            this.stateLength = stateLength;
            this.storyLength = storyLength;

            //creates database
            for (int i = 0; i < lines.Length - stateLength; i++) //beginning; end - length of state; increment;
            {
                string state = lines.Substring(i, stateLength);
                char next = lines[i+stateLength];
                //construct MarkovModel
                if (!database.Contains(state)) //if database doesn't have state
                {
                    database.Add(state, new MarkovEntry(state)); //create new one
                }
                database[state].Add(next); //add characters to the state in the database
            }
        }

        ///<summary>
        ///Records information in a LinkedList Symbol Table.
        ///</summary>
        public string CreateStory()
        {
            string story = ""; //stores the story as a whole
            string state = ""; //stores each state that program iterates through
            char c;            //stores each character generated according to each state

            //first state
            state = lines.Substring(0, stateLength);
            c = database[state].RandomLetter();
            story = story + state + c; //adds the beginning to the start of the story as well as the next character
            state = IncrementState(state, c);
            totalChar = totalChar + stateLength + 1;

            //loops until the next character is ".?!" or the count < storyLength
            for (int i = 0; i < storyLength; i++)
            {
                c = database[state].RandomLetter(); //gets a random letter
                story = story + c; //adds the letter to the story
                state = IncrementState(state, c); //moves the state to the next one forward
                totalChar++;
            }
            return story;
        }

        ///<summary>
        ///Adds the next character to the state, then cuts of the first character of the state to retain correct state length.
        ///</summary>
        public string IncrementState(string state, char c)
        {
            state += c;
            state = state.Substring(1);
            return state;
        }

        ///<summary>
        ///Returns totalChar.
        ///</summary>
        public int TotalChar
        {
            get {return totalChar;}
        }

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
    public class TreeMarkovModel
    {
        string lines;       //lines from the text
        int stateLength;    //state length
        int storyLength;    //story length
        int totalChar = 0;  //tracks total characters
        TreeSymbolTable<string, MarkovEntry> database;

        public TreeMarkovModel(string lines, int stateLength, int storyLength)
        {
            this.lines = lines;
            this.stateLength = stateLength;
            this.storyLength = storyLength;
            database = new TreeSymbolTable<string, MarkovEntry>();

            //creates database
            for (int i = 0; i < lines.Length - stateLength; i++) //beginning; end - length of state; increment;
            {
                string state = lines.Substring(i, stateLength);
                char next = lines[i+stateLength];
                //construct MarkovModel
                if (!database.ContainsKey(state))
                {
                    database.Add(state, new MarkovEntry(state));
                }
                database[state].Add(next);
            }
        }

        public string CreateStory()
        {
            string story = ""; //stores the story as a whole
            string state = ""; //stores each state that program iterates through
            char c;            //stores each character generated according to each state

            //first state
            state = lines.Substring(0, stateLength);
            c = database[state].RandomLetter();
            story = story + state + c; //adds the beginning to the start of the story as well as the next character
            state = IncrementState(state, c);
            totalChar = totalChar + stateLength + 1;

            //loops until the next character is ".?!" or the count < storyLength
            for (int i = 0; i < storyLength; i++)
            {
                c = database[state].RandomLetter(); //gets a random letter
                story = story + c; //adds the letter to the story
                state = IncrementState(state, c); //moves the state to the next one forward
                totalChar++;
            }
            return story;
        }

        public string IncrementState(string state, char c)
        {
            state += c;
            state = state.Substring(1); //gets all char after the first to retain correct stateLength length
            return state;
        }

        public int TotalChar
        {
            get {return totalChar;}
        }

        public override string ToString()
        {
            return database.Count + " nodes";
        }
    }

    ///<summary>
    ///Records information in .NET's Sorted Dictionary.
    ///</summary>
    public class DictMarkovModel
    {
        string lines;
        int stateLength;
        int storyLength;
        int totalChar = 0;  //tracks total characters

        SortedDictionary<string, MarkovEntry> database;

        public DictMarkovModel(string lines, int stateLength, int storyLength)
        {
            this.lines = lines;
            this.stateLength = stateLength;
            this.storyLength = storyLength;
            database = new SortedDictionary<string, MarkovEntry>();

            //creates database
            for (int i = 0; i < lines.Length - stateLength; i++) //beginning; end - length of state; increment;
            {
                string state = lines.Substring(i, stateLength);
                char next = lines[i+stateLength];
                //construct MarkovModel
                if (!database.ContainsKey(state))
                {
                    database.Add(state, new MarkovEntry(state));
                }
                database[state].Add(next);
            }
        }

        public string CreateStory()
        {
            string story = ""; //stores the story as a whole
            string state = ""; //stores each state that program iterates through
            char c;            //stores each character generated according to each state

            //first state
            state = lines.Substring(0, stateLength);
            c = database[state].RandomLetter();
            story = story + state + c; //adds the beginning to the start of the story as well as the next character
            state = IncrementState(state, c);
            totalChar = totalChar + stateLength + 1;

            //loops until the next character is ".?!" or the count < storyLength
            for (int i = 0; i < storyLength; i++)
            {
                c = database[state].RandomLetter(); //gets a random letter
                story = story + c; //adds the letter to the story
                state = IncrementState(state, c); //moves the state to the next one forward
                totalChar++;
            }
            return story;
        }

        public int TotalChar
        {
            get {return totalChar;}
        }

        public string IncrementState(string state, char c)
        {
            state += c;
            state = state.Substring(1); //gets all char after the first to retain correct stateLength length
            return state;
        }

        public override string ToString()
        {
            return database.Count + " nodes";
        }
    }
}
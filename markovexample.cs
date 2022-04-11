using System;
using System.IO;
using System.Collections.Generic;
using ListSymbolTable;

namespace Markov
{
    public class MarkovEntry
    {
        //class variables
        private string state;          //a particular group of characters. Example with a length of 6: "in a h"   *notice it counts spaces
        private int count;             //how many characters assigned to this state
        private MyList<char> suffixes; //where the characters are stored
        Random rnd;

        //creation of a specific state
        public MarkovEntry(string state) 
        {
            this.state = state;
            count = 0;
            suffixes = new MyList<char>();
            rnd = new Random();
        }

        //adds a character to the state and updates the amount of characters the state has
        public void Add(char ch)
        {
            count++;
            suffixes.Add(ch);
        }

        //gets a random letter from the state's assigned characters 
        public char RandomLetter()
        {
            return suffixes[rnd.Next(0, suffixes.Count)];
        }
    }

    public class ListMarkovModel
    {
        //class variables
        string lines;             //the lines of the text
        int markovDegreeNumber;   //how long the state should be 
        int storyLength;          //how long the story should be
        ListSymbolTable<string, MarkovEntry> database;  //where the states are stored

        public ListMarkovModel(string lines, int markovDegreeNumber, int storyLength) //user input is placed in the constructor
        {
            //user input assigned to class variables
            this.lines = lines;
            this.markovDegreeNumber = markovDegreeNumber;
            this.storyLength = storyLength;
            database = new ListSymbolTable<string, MarkovEntry>();

            //creates database using for loop to read all the characters in the text. It runs until it reaches the 
            //markovDegreeNumber + 1 of characters before the end. Example: 100 characters and the state is 5, it will end at 94
            //this way, the program does not go out of bounds by reading over the 100 characters
            for (int i = 0; i < lines.Length - markovDegreeNumber; i++) //beginning; end - length of state; increment;
            {
                //the program will read until 94. the state will look at 95-99, the character will look at 100
                string state = lines.Substring(i, markovDegreeNumber); //if the state is 5, then it will record 5 characters of the text
                char next = lines[i+markovDegreeNumber]; //this is why there is a +1. it reads one character in front of the state
                
                if (!database.Contains(state)) //if the database does not contain the state
                {
                    database.Add(state, new MarkovEntry(state)); //then add it
                }

                //assign the characters that come after the state, to the state. if the state is "in a h" the 
                //next characters could be 'a' or 'o' for "in a hat" or "in a house"
                //when the program sees this, it will assign the 'a' and the 'o' to the state
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

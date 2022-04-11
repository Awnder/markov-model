using System;
using System.Collections.Generic;
using ListSymbolTable;

public class MarkovModel
{
    //class variables
    string text;     //all the text
    int stateLength;  //how long the state should be 
    int storyLength;  //how long the story should be
    ListSymbolTable<string, MarkovEntry> database;  //where the states are stored

    public MarkovModel(string text, int stateLength, int storyLength) //the text plus the two user inputs placed in constructor
    {
        //user input assigned to class variables
        this.text = text;
        this.stateLength = stateLength;
        this.storyLength = storyLength;
        database = new ListSymbolTable<string, MarkovEntry>();

        //Creates database using for loop to read all the characters in the text. It runs until it reaches the 
        //stateLength + 1 of characters before the end. Example: 100 characters and the state is 5, it 
        //will end at 94. This way, the program does not go out of bounds by reading over the 100 characters

        for (int i = 0; i < text.Length - stateLength; i++) //beginning; end - length of state; increment;
        {
            //the loop will run until the 94th character. the state will read 95-99, the character will read 100
            string state = text.Substring(i, stateLength); //if the state is 5, then it will record 5 characters of the text
            char next = text[i+stateLength]; //this is why there is a +1. it reads one character in front of the state
            
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
}

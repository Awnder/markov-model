using System;
using System.Collections.Generic;
using ListSymbolTable;

public class MarkovModel
{
    string text;
    int stateLength;
    int storyLength;
    ListSymbolTable<string, MarkovEntry> database;

    public MarkovModel(string text, int stateLength, int storyLength)
    {
        this.text = text;
        this.stateLength = stateLength;
        this.storyLength = storyLength;
        database = new ListSymbolTable<string, MarkovEntry>();

        //loops through text to create states and assign characters to states
        for (int i = 0; i < text.Length - stateLength; i++)
        {
            string state = text.Substring(i, stateLength);
            char next = text[i+stateLength];
            
            //creating new state if doesn't exist
            if (!database.Contains(state))
            {
                database.Add(state, new MarkovEntry(state));
            }

            //assigning characters to state
            database[state].Add(next); 
        }
    }
}

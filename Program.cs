using System;
using MyListLibrary;
using ListSymbolTable;
using System.IO;
using System.Diagnostics;

namespace Markov
{
    public class Program
    {
        static void Main(string[] args)
        {
            //csc Program.cs mylistlibrary.cs listsymboltable.cs treesymboltable.cs markovmodel.cs 
            Stopwatch watch = new Stopwatch();
            string story = "";

            try
            {
                CheckArgs(args);
            } catch (ArgumentOutOfRangeException e) {
                Console.WriteLine(e.Message);
                return;
            } catch (ArgumentNullException e) {
                Console.WriteLine(e.Message);
                return;
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
                return;
            }

            // string lines = File.ReadAllText(args[0]); --- using this one instead
            // string[] lines = File.ReadAllLines(args[0]); --- can't use this b/c it doesn't retain newline characters, states not stored correctly

            watch.Start();
            string lines = "";
            try
            {
                lines = File.ReadAllText(args[0]);
            } catch {
                Console.WriteLine($"File \"{args[0]}\" does not exist");
                return;
            }
            ListMarkovModel listmodel = new ListMarkovModel(lines, int.Parse(args[1]), int.Parse(args[2]));
            story = listmodel.CreateStory();
            Console.WriteLine("Custom Linked List Symbol Table");
            Console.WriteLine($"Text Length: {listmodel.TotalChar} chars");
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine(story);
            watch.Stop();
            watch.Reset();

            Console.WriteLine("----------------------------------");
            
            watch.Start();
            try
            {
                lines = File.ReadAllText(args[0]);
            } catch {
                Console.WriteLine($"File \"{args[0]}\" does not exist");
                return;
            }
            TreeMarkovModel treemodel = new TreeMarkovModel(lines, int.Parse(args[1]), int.Parse(args[2]));
            story = treemodel.CreateStory();
            Console.WriteLine("Custom Binary Tree Symbol Table");
            Console.WriteLine($"Text Length: {treemodel.TotalChar} chars");
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine(story);
            watch.Stop();
            watch.Reset();

            Console.WriteLine("----------------------------------");

            watch.Start();
            try
            {
                lines = File.ReadAllText(args[0]);
            } catch {
                Console.WriteLine($"File \"{args[0]}\" does not exist");
                return;
            }
            DictMarkovModel dictmodel = new DictMarkovModel(lines, int.Parse(args[1]), int.Parse(args[2]));
            story = dictmodel.CreateStory();
            Console.WriteLine(".NET Sorted Dictionary");
            Console.WriteLine($"Text Length: {dictmodel.TotalChar} chars");
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine(story);
            watch.Stop();
        }

        static void CheckArgs(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Error: input must include three values: <string> fileName, <int> MarkovDegreeNumber, <int> storyLength");
            }
            args[0] = args[0].Trim();
            args[1] = args[1].Trim();
            args[2] = args[2].Trim();
            if (args[0].Equals("") || args[1].Equals("") || args[2].Equals(""))
            {
                throw new ArgumentNullException("Error: input cannot be blank spaces");
            }
            if (args[0].Length < 5 || !args[0].Substring(args[0].Length-4, 4).Equals(".txt"))
            {
                throw new ArgumentException("Error: <string> fileName must end in \".txt\" and not only be \".txt\"");
            }
            if (!int.TryParse(args[1], out int result))
            {
                throw new ArgumentException("Error: <int> MarkovDegreeNumber must be a number");
            }
            if (!int.TryParse(args[2], out result))
            {
                throw new ArgumentException("Error: <int> storyLength must be a number");
            }
        }
    }
}
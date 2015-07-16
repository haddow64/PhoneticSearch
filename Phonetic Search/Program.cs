using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Phonetic_Search
{
    /// <summary>
    ///     Phonetic Surname Search
    /// </summary>
    /// <remarks>
    ///     Modified version of the Soundex algorithm
    /// Rules:
    /// 1.  All non-alphabetic characters are ignored
    //  2.  Word case is not significant
    //  3.  After the first letter, any of the following letters are discarded: A, E, I, H, O, U, W, Y.
    //  4.  The following sets of letters are considered equivalent
    //      >  A, E, I, O, U
    //      >  C, G, J, K, Q, S, X, Y, Z
    //      >  B, F, P, V, W
    //      >  D, T
    //      >  M, N
    //      >  All others have no equivalent
    //  5.  Any consecutive occurrences of equivalent letters (after discarding letters in step 3) are considered as 
    //  a single occurrence
    /// </remarks>
    /// <author>
    ///     haddow64
    /// </author>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string[] splitUpNames = {};

            // List that loads contains the contents of the supplied .txt file
            IEnumerable<string> lines = new List<string>();

            // List containing the output names
            var userOutput = new List<string>();

            // Check for command line arguments or direct input if run from Visual Studio
            if (args.Length > 0)
            {
                splitUpNames = args;

                string fileRead;
                while ((fileRead = Console.ReadLine()) != null)
                {
                    lines = fileRead.Split(' ').ToList();
                }
            }
            else
            {
                Console.WriteLine("Please enter any surnames separated by a space \nand the the name of your file separated by a < symbol");
                Console.WriteLine("E.G. Name1 Name2 < surnames.txt \n");

                var inputData = Console.ReadLine();

                // Strip file name from input names
                var fileName = inputData;
                if (inputData != null)
                {
                    var nameIndex = inputData.LastIndexOf(" <", StringComparison.Ordinal);
                    if (nameIndex > 0)
                        inputData = inputData.Substring(0, nameIndex);
                }

                // Isolate the name of the input file
                if (fileName != null)
                {
                    var fileIndex = fileName.LastIndexOf("<", StringComparison.Ordinal);
                    if (fileIndex > 0)
                        fileName = fileName.Remove(0, fileIndex).Replace("<", "").Replace(" ", "");
                }

                if (inputData != null) splitUpNames = inputData.Split(' ');

                // List containing the contents of the text file
                if (fileName != null) lines = File.ReadLines(fileName);
                foreach (object l in lines)
                {
                    Console.WriteLine(l);
                }
            }

            /*
             * Main loop for the application 
             * First loop iterates over user input with the nested loop iterating over the list of names
             */
            foreach (var names in splitUpNames)
            {
                userOutput.Clear();

                try
                {
                    userOutput.AddRange(from item in lines let userInputSoundex = Soundex(names) let fileInputSoundex = Soundex(item)
                        where userInputSoundex == fileInputSoundex let storedName = names where storedName == names select item);

                    Console.WriteLine(names + ": " + string.Join(", ", userOutput.ToArray()));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to read file, did you spell it correctly?\n\nException was: " + e);
                }
            }
            Console.ReadLine();
        }


        /// <summary>
        ///     This algorithm is a modified version of the Soundex algorithm that does not
        ///     retain the leading letter and has different letter sets.
        ///     Part of below function based on existing soundex implementation found here:
        ///     http://www.techrepublic.com/blog/software-engineer/how-do-i-implement-the-soundex-function-in-c/#.
        /// </summary>
        /// <param name="inputNames">Called twice for user input and parsing of lines in .txt file</param>
        private static string Soundex(string inputNames)
        {
            var result = new StringBuilder();
            var previousNumber = "";

            var removeSpaces = inputNames.Replace(" ", "");

            // 1.  All non-alphabetic characters are ignored
            // 2.  Word case is not significant
            // Rule one implemented using regular expressions
            // Rule two implemented from RegexOptions.IgnoreCase
            // Slower than the C# implementation of IsLetterOrDigit but more commonly used because its easier to understand and maintain
            var onlyAlphabetic = Regex.Replace(removeSpaces, "[^a-z]", "", RegexOptions.IgnoreCase);

            // 3.  After the first letter, any of the following letters are discarded: A, E, I, H, O, U, W, Y.
            // Rule three stores the first letter in getFirstLetter
            // Then uses regular expressions to remove the defined letters and rejoins the first letter
            var getFirstLetter = onlyAlphabetic.Substring(0, 1);
            var removeLetters = Regex.Replace(onlyAlphabetic.Remove(0, 1), "[aeihouwy]", "", RegexOptions.IgnoreCase);
            inputNames = getFirstLetter + removeLetters;

            // 4.  The following sets of letters are considered equivalent
            // A, E, I, O, U
            // C, G, J, K, Q, S, X, Y, Z
            // B, F, P, V, W
            // D, T
            // M, N
            // All others have no equivalent
            if (inputNames.Length > 0)
            {
                for (var i = 1; i < inputNames.Length; i++)
                {
                    var evaluateLetter = inputNames.Substring(i, 1).ToLower();

                    string currentNumber;
                    if ("aeiou".IndexOf(evaluateLetter, StringComparison.Ordinal) > -1)
                        currentNumber = "1";
                    else if ("cgjkqsxyz".IndexOf(evaluateLetter, StringComparison.Ordinal) > -1)
                        currentNumber = "2";
                    else if ("bfpvw".IndexOf(evaluateLetter, StringComparison.Ordinal) > -1)
                        currentNumber = "3";
                    else if ("dt".IndexOf(evaluateLetter, StringComparison.Ordinal) > -1)
                        currentNumber = "4";
                    else if ("mn".IndexOf(evaluateLetter, StringComparison.Ordinal) > -1)
                        currentNumber = "5";
                    else
                        currentNumber = "6";

                    // 5.  Any consecutive occurrences of equivalent letters (after discarding letters in step 3) 
                    // are considered as a single occurrence
                    if (currentNumber != previousNumber)
                        result.Append(currentNumber);

                    if (result.Length == 4) break;

                    if (currentNumber != "")
                        previousNumber = currentNumber;
                }
            }

            if (result.Length < 4)
                result.Append(new string('0', 4 - result.Length));

            return result.ToString().ToUpper();
        }
    }
}
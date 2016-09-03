using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTest
{
    class Program
    {
        private static string _strToParse = "(id,created,employee(id,firstname,employeeType(id),lastname,Person(num,name,Manager(id,badge))),location)";
        static void Main(string[] args)
        {
            string stringToParse = string.Empty;
            if(args.Length > 0)
            {
                //we can be creating here and parse args with some regex etc.
                //but we will go just to start the flow as we have static string
                stringToParse = args[0].ToString();                
            }
            else
            {
                stringToParse = _strToParse;                
            }
            string[] ArrayToDisplay = ParseIt(stringToParse);
            Console.WriteLine("Parsing String \n");
            DisplayIt(ArrayToDisplay);
            Console.WriteLine("\n\n");           
            List<string> list = PrepareForSorting(ArrayToDisplay);
            list = SortedList(list);
            Console.WriteLine("Parsing & Sorting String \n");
            DisplayIt(list.ToArray());
            Console.ReadKey();
        }

        private static List<string> SortedList(List<string> list)
        {
            List<string> finalStrList = new List<string>();
            for(int x = 0; x < list.Count; x++)
            {
                string[] arr = list[x].Split('$');
                if(string.IsNullOrEmpty(arr.LastOrDefault()))
                {
                    finalStrList.Add(arr[0]);
                }
                else
                {
                    string previousParent = list[x - 1].Split('$')[1];
                    string previousChild = list[x - 1].Split('$')[0];
                    string currentParent = arr[1];
                    if(previousParent == currentParent)
                    {
                        int index = finalStrList.FindIndex(f => f == previousChild);
                        finalStrList.Insert(index + 1, arr[0]);
                    }
                    else
                    {
                        int index = finalStrList.FindIndex(f => f == arr[1]);
                        finalStrList.Insert(index + 1, arr[0]);
                    }
                }
            }

            return finalStrList;
        }

        private static List<string> PrepareForSorting(string[] ArrayToDisplay)
        {
            List<string> finalList = new List<string>();
            List<string> list = new List<string>();
            int maxOccurancesOfDash = 0;
            for (int x = 0; x < ArrayToDisplay.Count(); x++)
            {
                int count = ArrayToDisplay[x].Length - ArrayToDisplay[x].Replace("-", "").Length;                
                if(count > maxOccurancesOfDash)
                {
                    maxOccurancesOfDash = count;
                }
            }
            string parent = string.Empty;
            for(int x = 0; x <= maxOccurancesOfDash; x++)
            {                
                if(list.Count > 0)
                {
                    list.Sort();
                    finalList.AddRange(list);
                    list.Clear();
                    parent = string.Empty;
                }
                for(int y = 0; y < ArrayToDisplay.Count(); y++)
                {
                    int count = ArrayToDisplay[y].Length - ArrayToDisplay[y].Replace("-", "").Length;                   
                    if(x == count)
                    {
                        //Dashes coming in
                        if (count > 0)
                        {
                            if (parent == string.Empty)
                            {
                                parent = ArrayToDisplay[y - 1];
                            }
                            else
                            {
                                //previous elment's parent
                                if (list.LastOrDefault().IndexOf(ArrayToDisplay[y - 1]) > -1)
                                {
                                    //nothing to do
                                }
                                else
                                {
                                    int countOfDashInCurrentElement = ArrayToDisplay[y].Length - ArrayToDisplay[y].Replace("-", "").Length;
                                    int countOfDashInPreviousElement = ArrayToDisplay[y - 1].Length - ArrayToDisplay[y - 1].Replace("-", "").Length;
                                    if (countOfDashInCurrentElement > countOfDashInPreviousElement)
                                    {
                                        parent = ArrayToDisplay[y - 1];
                                    }                                    
                                }
                            }
                        }
                        //add marker to identify parents later on
                        list.Add(ArrayToDisplay[y] + "$" +  parent);
                    }
                }
            }
            list.Sort();
            finalList.AddRange(list);
            list.Clear();

            return finalList;
        }        

        private static string[] ParseIt(string stringToParse)
        {
            List<string> finalStringList = new List<string>();
            if(!string.IsNullOrEmpty(stringToParse))
            {
                //good one to parse                
                stringToParse = RemoveOpenAndCloseBracketsFromStartAndEnd(stringToParse);
                string[] fieldsFromStringToParse = stringToParse.Split(','); //parse by ,

                //we have

                //(id   --> Check for First Char ( and Last Char )
                //employee(
                //(id)
                //lastName)
                //location)
                
                int openingBracketCount = 0;
                foreach(string field in fieldsFromStringToParse)
                {
                    //int openingBracketPos = field.IndexOf("(");
                    if (field.IndexOf("(") > 0)
                    {                        
                        string[] childrenFieldsWithOpeningBracket = field.Trim().Split('(');
                        foreach (string str in childrenFieldsWithOpeningBracket)
                        {
                            string lf = string.Empty;
                            if (str == childrenFieldsWithOpeningBracket.LastOrDefault())
                            {
                                openingBracketCount++;
                                lf = ReplaceWithDashesHowManyTimes(openingBracketCount, str);
                                if(lf.IndexOf(")") > -1)
                                {
                                    int countOfOccurencesOfClosingBracket = lf.Length - lf.Replace(")", "").Length;
                                    lf = lf.Replace(")","");
                                    openingBracketCount = openingBracketCount - countOfOccurencesOfClosingBracket;
                                }
                            }
                            else
                            {
                                lf = ReplaceWithDashesHowManyTimes(openingBracketCount, str);
                            }                            
                            finalStringList.Add(lf);                            
                        }                        
                    }
                    else
                    {
                        string removedClosingBracketString = string.Empty;
                        string formattedField = string.Empty;
                        if (field.IndexOf(")") > -1)
                        {
                            int countOfOccurencesOfClosingBracket = field.Length - field.Replace(")", "").Length;
                            removedClosingBracketString = field.Replace(")", "");
                            formattedField = ReplaceWithDashesHowManyTimes(openingBracketCount, removedClosingBracketString);
                            openingBracketCount = openingBracketCount - countOfOccurencesOfClosingBracket;
                        }
                        else
                        {
                            removedClosingBracketString = field;
                            formattedField = ReplaceWithDashesHowManyTimes(openingBracketCount, removedClosingBracketString);
                        }
                        
                        finalStringList.Add(formattedField);
                    }
                }
            }
            else
            {
                Console.WriteLine("You passed to Parse String => " + stringToParse + " <= , but does not MEET THE CONTRACT");
            }

            return finalStringList.ToArray();
        }

        private static string RemoveOpenAndCloseBracketsFromStartAndEnd(string stringToParse)
        {
            if(!string.IsNullOrEmpty(stringToParse))
            {
                if(stringToParse.StartsWith("("))
                {
                    stringToParse = stringToParse.Substring(1, stringToParse.Length - 1);
                }
                if(stringToParse.EndsWith(")"))
                {
                    stringToParse = stringToParse.Substring(0, stringToParse.Length - 1);
                }
            }
            return stringToParse;
        }

        private static string ReplaceWithDashesHowManyTimes(int openBracketCount, string field)
        {
            StringBuilder stringWithDashes = new StringBuilder();
            for(int x = 1; x <= openBracketCount; x++)
            {
                stringWithDashes.Append("-");
            }

            return stringWithDashes.Append(field).ToString();
        }

        private static void DisplayIt(string[] fields)
        {
            foreach(string field in fields)
            {
                Console.WriteLine(field);
            }
        }
    }
}

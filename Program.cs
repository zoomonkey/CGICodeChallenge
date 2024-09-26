using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static System.Net.Mime.MediaTypeNames;
namespace Challenge;

class Program
{
    const string mykey = "laheravtam";

    static void Main(string[] args)
    {
        WriteOutFinalJson(LoadNameListWithKeyMatch());
    }

    static List<string> LoadNameListWithKeyMatch()
    {
        var namesInput = ReadJsonInput();

        // loop backwards so we can remove from our input list
        for (int i = namesInput.Count - 1; i >= 0; i--)
        {
            bool doAllMatch = true;

            // loop every leter and see if there's a match for each
            foreach (char c in namesInput[i])
            {
                if (!mykey.ContainsAnyCase(c))
                {
                    doAllMatch = false;
                    break;
                }
            }
            if (!doAllMatch)
            {
                namesInput.Remove(namesInput[i]);
            }
        }

        return namesInput;
    }

    static void WriteOutFinalJson(List<string> nameMatches)
    {
        bool isFirstTimeThru = true;
        string jsonOutput = string.Empty;
        var i = 0;

        foreach (string name in nameMatches)
        {
            // check if it's the last one and if it needs to be a pair instead of a single
            if (++i != nameMatches.Count)
            {
                if (isFirstTimeThru)
                {
                    jsonOutput += "['" + name + "'],";
                    isFirstTimeThru = false;
                }
                else
                {
                    jsonOutput += "['" + name + "']" + Environment.NewLine;
                    isFirstTimeThru = true;
                }
            }
            else
            {
                if (nameMatches.Count % 2 == 1)
                {
                    // here we are at the end and it's an odd number so we need a pair
                    var item = nameMatches[^2];
                    jsonOutput += "['" + item + "'],['" + name + "']";
                }
            }
        }

        string fileWithLocation = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        fileWithLocation += "\\output.json";
        File.WriteAllText(fileWithLocation, jsonOutput);

    }

    static List<string> ReadJsonInput()
    {
        try
        {
            var namesList = new List<string>();
            string fileWithLocation = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            fileWithLocation += "\\names.json";

            using (StreamReader r = new StreamReader(fileWithLocation))
            {
                string json = r.ReadToEnd();
                namesList = JsonConvert.DeserializeObject<List<string>>(json);
            }
            return namesList;
        }
        catch (Exception ex)
        {
            string dosomething = ex.Message;
            return null;
        }
    }
}

public static class MyStringExtensions
{
    public static bool ContainsAnyCase(this string haystack, char needle)
    {
        return haystack.IndexOf(needle, StringComparison.CurrentCultureIgnoreCase) != -1;
    }
}


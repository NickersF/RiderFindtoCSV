// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

// Rider Find Results to CSV File
// Author: Nichoals Fazzolari
// 6/13/YC125

// This parser utility takes in Rider IDE find results as a .txt
// and generates a CSV from the Path, File and Line position.

// The UI
Console.Write("Enter input file (.txt): ");
string? inputFile = Console.ReadLine();

if (string.IsNullOrEmpty(inputFile))
{
    Console.Write("No file provided. Shutting down.");
    Environment.Exit(0);
}

if (!inputFile.Contains(".txt"))
{
    inputFile += ".txt";
}

Console.Write("Enter output file name (.csv): ");
string? outputFile = Console.ReadLine();

if (string.IsNullOrEmpty(outputFile))
{
    Console.Write("Something went wrong.");
    Environment.Exit(0);
}

if (!outputFile.Contains(".csv"))
{
    outputFile += ".csv";
}

// The parser
try
{
    string pathString = "";
    string fileNameString = "";
    string positionString = "";
    List<string> textFileAsStringList = File.ReadAllLines(inputFile).ToList();
    List<string> parsedOutputFileStringList = new List<string>();

    // Start the CSV header. Could go further with this and implement it as cmd args (meh)
    parsedOutputFileStringList.Add("Path,File Name,Position");

    // Trim the first four lines that Rider generates as they do not contain meaningful data in this context.
    if (textFileAsStringList.Count >= 3)
    {
        textFileAsStringList.RemoveRange(0, 4);
    }
    else
    {
        parsedOutputFileStringList.Add("Parser Error - Line count out of bounds");
    }
    
    // Loop the list of lines and parse them
    for (int i = 0; i < textFileAsStringList.Count; i++)
    {
        string line = textFileAsStringList[i];
    
        // Remove leading spaces
        line = line.TrimStart();
        
        if (line.Contains("{"))
        {
            line = line.Replace("{", "");
        }
    
        // Check if the line contains numbers or the period character
        if (Regex.IsMatch(line, @"\d|\.")) // Updated condition
        {
            // Remove the parentheses and the contents inside them (regex style)
            line = Regex.Replace(line, @"\s*\([^()]*\)", string.Empty);
    
            // Remove the leading space before the opening parenthesis
            line = line.Replace(" (", "");
            
            textFileAsStringList[i] = line;
        }
        
        if (!Regex.IsMatch(line, @"\d|\."))
        {
            // Add a backslash at the end of the line
            line += "\\";
            
            textFileAsStringList[i] = line;
        }
    }
    
    // Parse into the CSV structure
    for (int i = 0; i < textFileAsStringList.Count; i++)
    {
        string line = textFileAsStringList[i];
        
        // Need to figure out how to make the pathString change when the path changes.
        // Need to generate some testing data and figure out the algo.
        if (line.Contains('\\'))
        {
            pathString += line;
        }

        if (!char.IsDigit(line[0]) && line.Contains('.'))
        {
            fileNameString = line;
        }
        
        if (char.IsDigit(line[0]))
        {
            int indexOfComma = line.IndexOf(',');

            if (indexOfComma != -1)
            {
                line = line.Substring(0, indexOfComma) + "...";
            }
            
            positionString = line;
        }

        if (fileNameString != "" && positionString != "")
        {
            string parsedLine = pathString + "," + fileNameString + "," + positionString;
        
            parsedOutputFileStringList.Add(parsedLine);
        }
    }

    // Write the List of strings to the output stream and exit
    File.WriteAllLines(outputFile, parsedOutputFileStringList);
}
catch (Exception ex)
{
    // The console exceptions messages are actually rather useful, using what the folks at MS implemented here.
    Console.WriteLine(ex.Message);
}

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
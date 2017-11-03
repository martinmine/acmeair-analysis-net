using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AcmeAirAnalysis
{
    class InvertedCSVFile
    {
        private List<List<string>> columns = new List<List<string>>();
        private int highestLine = 0;
        private const char delimiter = ' ';


        internal void AddList(List<string> lines)
        {
            columns.Add(lines);

            if (lines.Count > highestLine)
                highestLine = lines.Count;
        }

        internal void AddColumn(params object[] lines)
        {
            AddList(lines.Select(i => i.ToString()).ToList());
        }

        internal void WriteToFile(string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName, false))
            {
                for (int lineNumber = 0; lineNumber < highestLine; lineNumber++)
                {
                    foreach (var column in columns)
                    {
                        if (lineNumber < column.Count)
                        {
                            file.Write(column[lineNumber]);
                            file.Write(delimiter);
                        }
                        else
                        {
                            file.Write(delimiter);
                        }
                    }

                    file.WriteLine();
                }
            }
        }
    }
}

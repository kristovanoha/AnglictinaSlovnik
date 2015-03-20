using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTools
{
    public class SearchingFile
    {


        public SearchingFile(string nameFile)
        {

            analyze(nameFile);
        }

        private void analyze(string nameFile)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(nameFile);

            while (sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Console.WriteLine(line);
            }

            sr.Close();
        }

        

        public static void Test()
        {
            int a = 5;
        }
    }
}

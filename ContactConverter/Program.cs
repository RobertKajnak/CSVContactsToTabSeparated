using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactConverter
{
    class Program
    {
       
        static void Main(string[] args)
        {
            string inputFileName, outputFileName;

            Console.WriteLine("Please specify .csv filename");
            inputFileName = Console.ReadLine();
            Console.WriteLine("Please specify output .txt fileName");
            outputFileName = Console.ReadLine();

            Converter converter = new Converter();
            converter.Convert(inputFileName,outputFileName);

            long failure = new TabCountVerifier().Verify("out.txt");
            if ( failure == -1)
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("Failure at line " + failure + "!");
            }
            Console.ReadLine();
        }
    }
}

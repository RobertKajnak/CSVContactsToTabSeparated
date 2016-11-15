using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactConverter
{
    class TabCountVerifier
    {
        private const byte _r = 13;
        private const byte _n = 10;
        private const byte tab = 9;

        /// <summary>
        /// Return the line at wich the failure has occured ,-1 if no errors were found
        /// </summary>
        public long Verify(string FileName)
        {
     
            int tabCount = 0,tabCountLast = 0;
            byte b;
            bool isFirstLine = true;
            long lineCounter = 0;

            using (BinaryReader reader = new BinaryReader(File.Open(FileName, FileMode.Open)))
            {
                for (long i = 0; i < new FileInfo(FileName).Length; i++)
                {
                    b = reader.ReadByte();

                    if (b == tab)
                    {
                        tabCount++;
                    }
                    if (b == _n)
                    {
                        lineCounter++;
                        if (tabCountLast != tabCount && !isFirstLine)
                        {
                            return lineCounter;
                        }
                        tabCountLast = tabCount;
                        tabCount = 0;
                        isFirstLine = false;
                    }

                }
            }

            return -1;
        }
    }
}

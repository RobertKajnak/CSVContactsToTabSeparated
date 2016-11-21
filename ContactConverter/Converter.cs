using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactConverter
{
    class Converter
    {
        private const byte tab = 9;//11 can be used for testing, as that is visible in Notepad++
        private const byte hyphon1 = 226;
        private const byte hyphon2 = 128;
        private const byte hyphon3 = 157;
        private const byte paranthesis = 40;
        private const byte space = 32;
        private const byte _r = 13;
        private const byte _n = 10;
        private const byte colon = 59;


        private byte[] ProcessCommentIncluding(ref byte b, BinaryReader reader, BinaryWriter writer, ref long i)
        {
            return ProcessCommentIncluding(ref b, reader, writer,ref i, 0);
        }
        private byte[] ProcessCommentIncluding(ref byte b, BinaryReader reader, BinaryWriter writer, ref long i, byte lastChar)
        {
            writer.Write(tab);
            byte[] ba = new byte[256];
            
            int k = 0;
            if (lastChar != 0)
            {
                ba[0] = lastChar;
                k++;
            }

            while (b != colon)
            {
                ba[k++] = b;
                i++;
                b = reader.ReadByte();
            }
            return ba.Take(k).ToArray();
        }

        private byte[] ProcessComment(ref byte b, BinaryReader reader, BinaryWriter writer, ref long i)
        {
            writer.Write(tab);
            byte[] ba = new byte[256];
            int k = 0;
            b = reader.ReadByte();
            i++;
            while (b == space)
            {
                i++;
                b = reader.ReadByte();
            }
            while (b != colon)
            {
                ba[k++] = b;
                i++;
                b = reader.ReadByte();
            }
            return ba.Take(k).ToArray();
        }

        public Converter ()
        {
        }

        public void Convert(string CSVFileName, string txtFileName)
        {
            string fileNameRead = CSVFileName;
            string fileNameWrite = txtFileName;
            byte b, bOld, lastCapital=0;
            long length = new FileInfo(fileNameRead).Length;
            bool isNameProcessed = false;
            bool isCommentProcessed = false;
            


            byte[] comment = null;
            using (BinaryReader reader = new BinaryReader(File.Open(fileNameRead, FileMode.Open)))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileNameWrite, FileMode.Create)))
                {
                    b = 0;
                    for (long i = 0; i < length; i++)
                    {
                        bOld = b;
                        b = reader.ReadByte();
                        switch (b)
                        {
                            ///name separator
                            case (space):
                                if (!isNameProcessed)
                                {
                                    writer.Write(tab);//tab
                                    isNameProcessed = true;
                                }
                                else
                                {
                                    writer.Write(b);
                                }
                                break;

                            ////comment separators
                            case (paranthesis):
                                comment = ProcessComment(ref b, reader, writer, ref i);
                                comment = comment.Take(comment.Length - 1).ToArray();
                                if (!isNameProcessed)
                                {
                                    writer.Write(tab);
                                    isNameProcessed = true;
                                }
                                isCommentProcessed = true;

                                break;
                            case (hyphon1):
                                reader.ReadByte();
                                reader.ReadByte();
                                i += 2;

                                comment = ProcessComment(ref b, reader, writer, ref i);
                                if (!isNameProcessed)
                                {
                                    writer.Write(tab);
                                    isNameProcessed = true;
                                }
                                isCommentProcessed = true;
                                break;

                            case (44)://,
                            case (45)://-
                                if (!isCommentProcessed)
                                {
                                    comment = ProcessComment(ref b, reader, writer, ref i);
                                    isCommentProcessed = true;
                                    if (!isNameProcessed)
                                    {
                                        writer.Write(tab);
                                        isNameProcessed = true;
                                    }
                                }
                                else
                                {
                                    writer.Write(b);
                                }
                                break;
                            ///field separator
                            case (colon): //;

                                if (!isNameProcessed)
                                {
                                    writer.Write(tab);
                                    isNameProcessed = true;
                                }
                                isCommentProcessed = true;
                                writer.Write(tab);//tab
                                break;

                            ///contact separator
                            case (_r)://\r
                                writer.Write(tab);//tab
                                if (comment != null)
                                {
                                    writer.Write(comment);
                                }
                                writer.Write((byte)13);//\r
                                writer.Write((byte)10);//\n

                                isNameProcessed = false;
                                isCommentProcessed = false;
                                comment = null;
                                break;
                            case (_n)://\n
                                break;

                            ///ignored characters
                            case (tab):
                                break;

                            ///default
                            default:
                                if (!isCommentProcessed && isNameProcessed && (b >= 97 && b <= 122) && bOld == space)//starts with small leter
                                {
                                    comment = ProcessCommentIncluding(ref b, reader, writer, ref i);
                                    if (!isNameProcessed)
                                    {
                                        writer.Write(tab);
                                        isNameProcessed = true;
                                    }
                                    isCommentProcessed = true;
                                }
                                else if (!isCommentProcessed && isNameProcessed && (b >= 65 && b <= 90) && (bOld>=65 && bOld<=90) )//2 capitals after one another
                                {
                                    lastCapital = 0;
                                    comment = ProcessCommentIncluding(ref b, reader, writer, ref i,bOld);
                                    isCommentProcessed = true;
                                    if (!isNameProcessed)
                                    {
                                        writer.Write(tab);
                                        isNameProcessed = true;
                                    }
                                }
                                else
                                {
                                    if (isNameProcessed && (b >= 65 && b <= 90 ) &&!(bOld>65 && bOld<=90))
                                    {
                                        lastCapital = b;
                                    }
                                    else {
                                        if (lastCapital != 0)
                                        {
                                            writer.Write(lastCapital);
                                            lastCapital = 0;
                                        }
                                        writer.Write(b);
                                    }
                                }
                                
                                break;
                        }
                    }
                    if (b != _n)
                    {
                        writer.Write(tab);//tab
                        if (comment != null)
                        {
                            //writer.Write(comment);
                            for (int k = 0; k<comment.Length; k++)
                            {
                                writer.Write(comment[k]);
                            }
                        }
                    }

                }

            }
        }

    }
}

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
        const byte tab = 11;//9;
        const byte hyphon1 = 226;
        const byte hyphon2 = 128;
        const byte hyphon3 = 157;
        const byte paranthesis = 40;
        const byte space = 32;
        const byte _r = 13;
        const byte _n = 10;
        const byte colon = 59;

        static byte[] ProcessComment(ref byte b, BinaryReader reader, BinaryWriter writer, ref long i)
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

        static void Main(string[] args)
        {
            string fileNameRead = "table.csv";
            string fileNameWrite = "out.txt";
            char c;
            byte b,bOld;
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
                                
                                 break;
                            case (hyphon1):
                                reader.ReadByte();
                                reader.ReadByte();
                                i += 2;

                                comment = ProcessComment(ref b, reader, writer, ref i);
                                break;

                            case (44)://,
                            case (45)://-
                                if (!isCommentProcessed)
                                {
                                    comment = ProcessComment(ref b, reader, writer, ref i);
                                    isCommentProcessed = true;
                                }
                                else
                                {
                                    writer.Write(b);
                                }
                                break;
                                case (46):
                    break;
                            ///field separator
                            case (colon): //;
                                if (!isNameProcessed)
                                {
                                    writer.Write(tab);//tab
                                }

                                isNameProcessed = true;
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
                                if (!isCommentProcessed && isNameProcessed && b>=97 && b <= 122)
                                {
                                    comment = ProcessComment(ref b, reader, writer, ref i);
                                    isCommentProcessed = true;
                                }
                                writer.Write(b);
                                break;
                        }
                    }
                    if (b != _n)
                    {
                        writer.Write(tab);//tab
                        if (comment != null)
                        {
                            writer.Write(comment);
                        }
                    }
                }

            }
        }
    }
}

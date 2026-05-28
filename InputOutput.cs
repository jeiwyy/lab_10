using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    struct TextPosition
    {
        public uint lineNumber;
        public byte charNumber;

        public TextPosition(uint ln = 0, byte c = 0)
        {
            lineNumber = ln;
            charNumber = c;
        }
    }

    struct Err
    {
        public TextPosition errorPosition;
        public byte errorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            this.errorPosition = errorPosition;
            this.errorCode = errorCode;
        }
    }

    class InputOutput
    {
        private const byte ERRMAX = 9;

        public static char Ch { get; set; }
        public static TextPosition positionNow = new TextPosition(0, 0);
        public static List<Err> err = new List<Err>();
        public static bool IsEndOfFile { get; private set; } = false;

        private static string line = "";
        private static int lastInLine = 0;
        private static StreamReader File { get; set; }
        private static uint errCount = 0;

        public static void Init(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"Ошибка: Файл {filePath} не найден.");
                return;
            }

            File = new StreamReader(filePath);
            errCount = 0;
            IsEndOfFile = false;
            positionNow = new TextPosition(1, 0);
            err = new List<Err>();

            if (!File.EndOfStream)
            {
                line = File.ReadLine();
                line += " ";
                lastInLine = line.Length - 1;
                Ch = line[0];
            }
            else
            {
                IsEndOfFile = true;
                Ch = '\0';
            }
        }

        public static void NextCh()
        {
            if (IsEndOfFile)
            {
                Ch = '\0';
                return;
            }

            if (positionNow.charNumber >= lastInLine)
            {
                ListThisLine();
                if (err.Count > 0)
                {
                    ListErrors();
                }

                ReadNextLine();

                if (!IsEndOfFile)
                {
                    positionNow.lineNumber++;
                    positionNow.charNumber = 0;
                    Ch = line[0];
                }
            }
            else
            {
                positionNow.charNumber++;
                Ch = line[positionNow.charNumber];
            }
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            if (err.Count <= ERRMAX)
            {
                Err e = new Err(position, errorCode);
                err.Add(e);
            }
        }

        private static string ErrorDescr(byte errorCode)
        {
            string err = "";
            switch (errorCode)
            {
                case 4:
                    err += "4 - отстутствие ';'";
                    break;
                case 10:
                    err += "10 - переменной типа int не может быть присвоено string";
                    break;
                default: 
                    err += "Неизвестная ошибка.";
                    break;
            }
            return err;
        }

        private static void ListThisLine()
        {
            Console.WriteLine($"{positionNow.lineNumber.ToString().PadLeft(4)} | {line.TrimEnd()}");
        }

        private static void ReadNextLine()
        {
            if (!File.EndOfStream)
            {
                line = File.ReadLine();
                line += " ";
                lastInLine = line.Length - 1;
                err = new List<Err>();
            }
            else
            {
                IsEndOfFile = true;
                Ch = '\0';
                File.Close();
                End();
            }
        }

        private static void End()
        {
            Console.WriteLine("=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=");
            Console.WriteLine($"Компиляция завершена. Всего ошибок обнаружено: {errCount}!");
            Console.WriteLine("=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=");
        }

        private static void ListErrors()
        {
            foreach (Err item in err)
            {
                ++errCount;
                string s = "  !";
                if (errCount < 10)
                {
                    s += "0";
                }
                s += $"{errCount}!";

                int totalIndent = 6 + item.errorPosition.charNumber;
                s = s.PadRight(totalIndent) + $"^ ошибка {ErrorDescr(item.errorCode)}";
                Console.WriteLine(s);
            }
        }
    }
}

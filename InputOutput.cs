using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    struct TextPosition
    {
        public uint LineNumber { get; set; }
        public byte CharNumber { get; set; }

        public TextPosition(uint lineNumber = 0, byte charNumber = 0)
        {
            LineNumber = lineNumber;
            CharNumber = charNumber;
        }
    }

    struct Err
    {
        public TextPosition ErrorPosition { get; set; }
        public byte ErrorCode { get; set; }

        public Err(TextPosition errorPosition, byte errorCode)
        {
            ErrorPosition = errorPosition;
            ErrorCode = errorCode;
        }
    }

    class InputOutput
    {
        private const byte ErrMax = 9;

        public static char Ch { get; set; }
        public static TextPosition PositionNow { get; set; }
        public static List<Err> Errors { get; private set; } 
        public static bool IsEndOfFile { get; private set; }

        private static string _line;
        private static int _lastInLine;
        private static StreamReader _file;
        private static uint _errCount;

        public static void Init(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Ошибка: Файл {filePath} не найден.");
                return;
            }

            _file = new StreamReader(filePath);
            _errCount = 0;
            IsEndOfFile = false;
            PositionNow = new TextPosition(1, 0);
            Errors = new List<Err>();

            if (!_file.EndOfStream)
            {
                _line = _file.ReadLine();
                _line += " ";
                _lastInLine = _line.Length - 1;
                Ch = _line[0];
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

            if (PositionNow.CharNumber >= _lastInLine)
            {
                ListThisLine();
                if (Errors.Count > 0)
                {
                    ListErrors();
                }

                ReadNextLine();

                if (!IsEndOfFile)
                {
                    var curPosition = PositionNow;
                    curPosition.LineNumber++;
                    curPosition.CharNumber = 0;
                    PositionNow = curPosition;
                    
                    Ch = _line[0];
                }
            }
            else
            {
                var curPosition = PositionNow;
                curPosition.CharNumber++;
                PositionNow = curPosition;
                
                Ch = _line[PositionNow.CharNumber];
            }
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            if (Errors.Count <= ErrMax)
            {
                var e = new Err(position, errorCode);
                Errors.Add(e);
            }
        }

        private static string ErrorDescr(byte errorCode)
        {
            string description = "";
            switch (errorCode)
            {
                case 4:
                    description += "4 - отсутствие ';'";
                    break;
                case 10:
                    description += "10 - супер ошибка";
                    break;
                default: 
                    description += "Неизвестная ошибка.";
                    break;
            }
            return description;
        }

        private static void ListThisLine()
        {
            Console.WriteLine($"{PositionNow.LineNumber.ToString().PadLeft(4)} | {_line.TrimEnd()}");
        }

        private static void ReadNextLine()
        {
            if (!_file.EndOfStream)
            {
                _line = _file.ReadLine();
                _line += " ";
                _lastInLine = _line.Length - 1;
                Errors = new List<Err>();
            }
            else
            {
                IsEndOfFile = true;
                Ch = '\0';
                _file.Close();
                End();
            }
        }

        private static void End()
        {
            Console.WriteLine("=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=");
            Console.WriteLine($"Компиляция завершена. Всего ошибок обнаружено: {_errCount}!");
            Console.WriteLine("=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=");
        }

        private static void ListErrors()
        {
            foreach (var item in Errors)
            {
                ++_errCount;
                string s = "  !";
                if (_errCount < 10)
                {
                    s += "0";
                }
                s += $"{_errCount}!";

                int totalIndent = 6 + item.ErrorPosition.CharNumber;
                s = s.PadRight(totalIndent) + $"^ ошибка {ErrorDescr(item.ErrorCode)}";
                Console.WriteLine(s);
            }
        }
    }
}
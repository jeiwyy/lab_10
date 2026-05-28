﻿using System;
using System.IO;

namespace Compiler
{
    class Program
    {
        static void Main()
        {
            string Filename = "test.pas";
            Console.WriteLine("Тестирование модуля ввода-вывода");
            Console.WriteLine($"Загрузка файла: {Filename}\n");

            InputOutput.Init(Filename);

            while (!InputOutput.IsEndOfFile)
            {
                if (InputOutput.positionNow.lineNumber == 1
                && InputOutput.positionNow.charNumber == 2)
                {
                    InputOutput.Error(10, InputOutput.positionNow);
                }

                if (InputOutput.positionNow.lineNumber == 6
                && InputOutput.positionNow.charNumber == 7)
                {
                    InputOutput.Error(4, InputOutput.positionNow);
                }

                InputOutput.NextCh();
            }
        }
    }
}
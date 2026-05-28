﻿using System;

namespace Compiler
{
    class Program
    {
        static void Main()
        {
            string filename = "test.pas";
            Console.WriteLine("Тестирование модуля ввода-вывода");
            Console.WriteLine($"Загрузка файла: {filename}\n");

            InputOutput.Init(filename);

            while (!InputOutput.IsEndOfFile)
            {
                if (InputOutput.PositionNow.LineNumber == 1 &&
                    InputOutput.PositionNow.CharNumber == 2)
                {
                    InputOutput.Error(10, InputOutput.PositionNow);
                }

                if (InputOutput.PositionNow.LineNumber == 6 &&
                    InputOutput.PositionNow.CharNumber == 7)
                {
                    InputOutput.Error(4, InputOutput.PositionNow);
                }

                InputOutput.NextCh();
            }
        }
    }
}
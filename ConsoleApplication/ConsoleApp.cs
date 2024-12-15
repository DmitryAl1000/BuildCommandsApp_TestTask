using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using Domain;

namespace ConsoleApplication
{
    public class ConsoleApp
    {
        private const string HEADER_TEXT = "/exit - для выхода \n" +
                                           "/print - для печати всех доступных задач \n" +
                                             "\"Название задачи\" - Для запуска задачи\n";
        private const string EXIT_COMMAND = "/exit";
        private const string PRINT_COMMAND = "/print";

        private ConcurrentDictionary<string, Target> _targetsDictionary;
        private TargetLogic _targetLogic;

        public ConsoleApp(ConcurrentDictionary<string, Target> targetsDictionary)
        {
            this._targetsDictionary = targetsDictionary;
            _targetLogic = new TargetLogic(_targetsDictionary);
        }

        public void Run()
        {
            Console.WriteLine(HEADER_TEXT);
            //Запускаем условный интерфейс ввода задач
            StartLoop();
        }

        private void StartLoop()
        {
            string? command = string.Empty;
            
            while (command != EXIT_COMMAND)
            {
                EnterTargetMessege();
                command = Console.ReadLine();
                if (command is null) continue;
                if (command == EXIT_COMMAND) break;
                if (command == PRINT_COMMAND)
                {
                    _targetLogic.PrintAllTargets();
                    continue;
                }
                if (_targetsDictionary.ContainsKey(command.ToLower()))
                {
                    TryExecuteCommand(command);
                }
                else
                {
                    ErrorMessege($"!!!Задача не найдена!!!");
                    continue;
                }
            }
        }
        private void TryExecuteCommand(string? command)
        {
            if (command is null) return;

            try
            {
                _targetLogic.Execute(command);
            }
            catch (ArgumentException ex)
            {
                ErrorMessege($"Ошибка выполнения: {ex.Message}");
            }
        }
        private void ErrorMessege(string messege)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(messege);
            Console.ResetColor();
        }
        private void EnterTargetMessege()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Введите задачу");
            Console.ResetColor();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Domain;

namespace Application
{
    public class TargetLogic
    {
        ConcurrentDictionary<string, Target> _dictionaryOfTargets;

        Stack<Target> _resultTargetsStack; // результирующий стек задач который надо выполнить

        HashSet<string> _resultTargetHash; // главные задачи. Они не должны вызываться как зависимые у других задач. Иначе будет зацикливание



        public TargetLogic(ConcurrentDictionary<string, Target> dictionary)
        {
            this._dictionaryOfTargets = dictionary;
            _resultTargetsStack = new();
            _resultTargetHash = new();
        }

        /// <summary>
        /// Напечатать все имеющиеся задачи и действия
        /// </summary>
        public void PrintAllTargets()
        {
            foreach (var target in _dictionaryOfTargets.Values)
            {
                Console.WriteLine($"======{target.TargetName}======");

                foreach (var dependentTarget in target.DependentTargetsNames)
                {
                    Console.WriteLine($"Зависят: {dependentTarget}");
                }

                foreach (var action in target.Actions)
                {
                    Console.WriteLine($"Действия: {action}");
                }
            }
        }

        /// <summary>
        /// Выполнить все действия в данный задаче. Выполнить все задачи от которых задача зависит.
        /// </summary>
        /// <param name="targetName"></param>
        public void Execute(string targetName)
        {
            //находим все задачи и задачи задач
            FillStackOfTargetsNoRecurtion(targetName);
            //Выполняем в обратном порядке все действия всех задач
            ExecuteAllActionsFromStaсkOftTrgets();
        }

        //без рекурсии находим все задачи и задачи задач, записываем в стек
        private void FillStackOfTargetsNoRecurtion(string targetName)
        {
            ExeptionIfTargetNotInDictionary(targetName);

            _resultTargetsStack.Clear(); //Очищаем стек и хеш от прошлых поисков
            _resultTargetHash.Clear();

            Stack<Target> targetsInWork = new();
            Target firstTarget = _dictionaryOfTargets[targetName.ToLower()]; // Первая задача для работы
            targetsInWork.Push(firstTarget);

            while (targetsInWork.Count > 0)
            {
                Target target = targetsInWork.Pop();
                if (_resultTargetHash.Contains(target.TargetName)) continue;  //Если задача уже есть в хешсете значит она считается выполненной. Ничего делать с ней не надо
                _resultTargetsStack.Push(target);                 //Добавялем задачу в список на выполнение
                _resultTargetHash.Add(target.TargetName); // добавляем задачу в хешсет для быстрого поиска

                foreach (var dependentTargetName in target.DependentTargetsNames)
                {
                    ExeptionIfTargetNotInDictionary(dependentTargetName);
                    if (_resultTargetHash.Contains(dependentTargetName)) // Если новая задача уже есть в хешсет, значит у нас зацикливание
                    {
                        ExeptionLooping(dependentTargetName);
                    }

                    Target dependentTarget = _dictionaryOfTargets[dependentTargetName.ToLower()];
                    targetsInWork.Push(dependentTarget);
                }
            }
        }

        private void ExeptionLooping(string dependenTargetName)
        {
            StringBuilder errorMessege = new StringBuilder();
            foreach (var targetForError in _resultTargetsStack)
            {
                errorMessege.Append($" <= {targetForError.TargetName}");

                if (targetForError.TargetName == dependenTargetName) break;
            }
            throw new ArgumentException($"Произошло зацикливание на задачу:\n{dependenTargetName}{errorMessege}");
        }
        private void ExeptionIfTargetNotInDictionary(string targetName)
        {
            if (!IsTargetInDictionary(targetName))
            {
                if (_resultTargetsStack.Count > 0)
                {
                    var lastTarget = _resultTargetsStack.Pop();
                    string targetWhitherror = $"Неверная зависимость у задачи: {lastTarget.TargetName}\n";
                    throw new ArgumentException($"Среди задач нет: {targetName} \n{targetWhitherror}");
                }
                else
                {
                    throw new ArgumentException($"Среди задач нет: {targetName}");
                }
            }
        }

        //Выполняем все действия(вывод на консоль) которые помещены в стек задач
        private void ExecuteAllActionsFromStaсkOftTrgets()
        {
            foreach (var target in _resultTargetsStack)
            {
                ExecuteActions(target);
            }
        }
        //Выполняем действия для одно задачи
        private void ExecuteActions(Target target)
        {
            foreach (var action in target.Actions)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Action: {action} ({target.TargetName})");
                Console.ResetColor();
            }
        }
        // Эта задача есть в словаре? 
        private bool IsTargetInDictionary(string targetName)
        {
            if (_dictionaryOfTargets.ContainsKey(targetName.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

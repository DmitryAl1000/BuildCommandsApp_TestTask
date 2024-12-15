using Domain;
using System;
using System.Collections.Concurrent;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Application
{
    public class ParseMakeFile
    {
        private readonly string[] _makeFileLines;

        private const char TARGET_NAME_SEPARATOR = ':';
        private readonly char[] COMMAND_TABS_SIMBOlS = { ' ', '\\' };
        private readonly string COMMAND_TABS_FOR_DELETE = "\\t";


        private ConcurrentDictionary<string, Target> _resultDictionary = new();
        private BlockingCollection<string> _duplicatesTargets = new();
        public ParseMakeFile(string[] lines)
        {
            if (lines.Length != 0 && lines != null)
            {
                this._makeFileLines = lines;
            }
            else   
            {
                throw new ArgumentException($"Данных нет, нечего парсить");
            }
        }

        public ConcurrentDictionary<string, Target> GetConcurrentDictionaryParallel()
        {
            List<List<string>> listOfTargetsLInes = ParseToList();
            ParallelAddTargetsToDictionary(listOfTargetsLInes);
            return _resultDictionary;
        }

        public ConcurrentDictionary<string, Target> GetConcurrentDictionary()
        {
            Parse();
            return _resultDictionary;
        }



        /// <summary>
        /// Обычный синхронный парсинг. Проходим по массиву строк, добавляем элементы в словарь
        /// </summary>
        private void Parse()
        {
            List<string> singleTarget = new();
            List<Task> tasks = new();
            // строка в файле
            for (int i = 0; i < _makeFileLines.Length ; i++)
            {
                //чистим от табов
                _makeFileLines[i] = ClerFromTabs(_makeFileLines[i]);
                //выделяем блок Задача и ее команды для каждой задачи
                if (IsItLineWhithTarget(_makeFileLines[i]) && i != 0 )
                {
                    AddTargetToDictionary(singleTarget);
                    singleTarget.Clear();
                }
                singleTarget.Add(_makeFileLines[i]);
            }
            //Добавляем последний
            AddTargetToDictionary(singleTarget);
        }

        /// <summary>
        /// Добавляем в List несколько строк относящихся к одному target. Дабы затем распаралелить процесс парсинга
        /// </summary>
        /// <returns></returns>
        private List<List<string>> ParseToList()
        {
            List<List<string>> listOfSingleTarget = new();
            List<string> singleTarget = new();
            List<Task> tasks = new();

            // строка в файле
            for (int i = 0; i < _makeFileLines.Length; i++)
            {
                //чистим от табов
                _makeFileLines[i] = ClerFromTabs(_makeFileLines[i]);
                //выделяем блок Задача и ее команды для каждой задачи
                if (IsItLineWhithTarget(_makeFileLines[i]) && i != 0)
                {
                    listOfSingleTarget.Add(singleTarget);
                    singleTarget = new();
                }
                singleTarget.Add(_makeFileLines[i]);
            }
            //Добавляем последний
            listOfSingleTarget.Add(singleTarget);
            return listOfSingleTarget;
        }

        private void ParallelAddTargetsToDictionary(List<List<string>> listOfSingleTarget)
        {
            ParallelLoopResult result = 
                Parallel.ForEach<List<string>>(listOfSingleTarget,AddTargetToDictionary
            );
        }

        private bool IsItLineWhithTarget(string line)
        {
            if(line.Length == 0) return false;
            foreach (var tabSimbol in COMMAND_TABS_SIMBOlS)
            {
                if (line[0] == tabSimbol)
                    return false;
            }
            return true;
        }
        private void AddTargetToDictionary(List<string> targetInLInes)
        {
            if (targetInLInes[0] is null) return;
            string targetName = GetTargetName(targetInLInes[0]);

            string[] dependentTargetsNames = GetDependentTargetsNames(targetInLInes[0], targetName);
            string[] actions = GetActions(targetInLInes);


            Target target = new Target(targetName, dependentTargetsNames, actions);
            // Добавляем в словарь
            bool addedSuccessfully = _resultDictionary.TryAdd(targetName, target);
            if (!addedSuccessfully)
            {
                _duplicatesTargets.TryAdd(targetName);
            }

        }
        private string GetTargetName(string line)
        {
            if (line is null) return string.Empty;
            int separatorPosition = line.IndexOf(TARGET_NAME_SEPARATOR);
            if (separatorPosition > 0)
            {
                return line.Substring(0, separatorPosition).ToLower();
            }
            else
            {
                return line.ToLower();
            }
        }
        private string[] GetDependentTargetsNames(string line, string targetName)
        {
            //обрезаем из строки имя таргета. +1 чтобы исключить разделитель
            if (line.Contains(TARGET_NAME_SEPARATOR))
            {
                line = line.Substring(targetName.Length + 1, line.Length - 1 - targetName.Length).ToLower();
                return line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            return []; 
        }
        private string[] GetActions(List<string> targetInLInes)
        {
            string[] actions = new string[targetInLInes.Count - 1];
            //первую строчку пропускаем там задачи и зависимые задачи
            for (int i = 1; i < targetInLInes.Count; i++)
            {
                targetInLInes[i] = ClearFromSpases(targetInLInes[i]);
                actions[i - 1] = targetInLInes[i];

            }
            return actions;
        }
        private string ClearFromSpases(string line)
        {
            return line.Replace(" ", "");
        }
        private string ClerFromTabs(string line)
        {
            return line.Replace(COMMAND_TABS_FOR_DELETE," ");
        }
    }
}

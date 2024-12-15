using Application;
using Domain;
using System.Collections.Concurrent;
using System.Diagnostics;


namespace SimpleSpeedTests
{
    public class SimpleSpeedTest
    {
        private ConcurrentDictionary<string, Target> _targetsDictionary = new();

        readonly string _makeFileName;
        readonly string _settingsPath;

        List<string> _report;
        Stopwatch _stopWatch;


        string[] _lines;

        public SimpleSpeedTest(string makeFileName)
        {
            this._makeFileName = makeFileName;
            _report = new();
            _stopWatch = new();
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _makeFileName);

            _lines = [];
        }

        public List<string> RunTests()
        {
            TextFromFile();
            TextFromFileAsync();
            LinesFromFile();
            LinesFromFileAsync();
            ParseFromLines();
            ParseFromLinesParallel();
            ExecuteTarget();

            return _report;
        }

        public void PrintReport()
        {
            foreach (var item in _report)
            {
                Console.WriteLine(item);
            }
        }


        private void TextFromFile()
        {
            _stopWatch.Restart();
            var text = File.ReadAllText(_settingsPath);

            _stopWatch.Stop();
            _report.Add($"Текст из файла синхронно:{_makeFileName} " + _stopWatch.ElapsedMilliseconds + "ms");
        }

        private void TextFromFileAsync()
        {
            _stopWatch.Restart();
            var textAsync = File.ReadAllTextAsync(_settingsPath).Result;
            _stopWatch.Stop();
            _report.Add($"Текст из файла Aсинхронно:{_makeFileName} " + _stopWatch.ElapsedMilliseconds + "ms");
        }

        private void LinesFromFile()
        {
            _stopWatch.Restart();
            var linesSinc = File.ReadAllLines(_settingsPath);
            _stopWatch.Stop();
            _report.Add($"Массив строк из файла обычный:{_makeFileName} " + _stopWatch.ElapsedMilliseconds + "ms");
        }


        private void LinesFromFileAsync()
        {
            _stopWatch.Restart();
            _lines = File.ReadAllLinesAsync(_settingsPath).Result;
            _stopWatch.Stop();
            _report.Add($"Массив строк из файла асинхронно:{_makeFileName} " + _stopWatch.ElapsedMilliseconds + "ms");
        }

        private void ParseFromLines()
        {
            _stopWatch.Restart();
            ParseMakeFile parseMakeFileSinc = new ParseMakeFile(_lines);
            _targetsDictionary = parseMakeFileSinc.GetConcurrentDictionary();
            _stopWatch.Stop();
            _report.Add($"Парсинг в словарь обычный:{_makeFileName} " + _stopWatch.ElapsedMilliseconds + "ms");
        }

        private void ParseFromLinesParallel()
        {
            _stopWatch.Restart();
            ParseMakeFile parseMakeFile = new ParseMakeFile(_lines);
            _targetsDictionary = parseMakeFile.GetConcurrentDictionaryParallel();
            _stopWatch.Stop();
            _report.Add($"Парсинг в словарь параллельный:{_makeFileName} " + _stopWatch.ElapsedMilliseconds + "ms");
        }

        private void ExecuteTarget()
        {
            _stopWatch.Restart();
            TargetLogic targetLogic = new TargetLogic(_targetsDictionary);

            string targetName = "target1";

            targetLogic.Execute(targetName);

            _stopWatch.Stop();
            _report.Add($"Скорость выполнения {targetName} из файла {_makeFileName} " + _stopWatch.ElapsedMilliseconds + "ms");
        }
    }
}

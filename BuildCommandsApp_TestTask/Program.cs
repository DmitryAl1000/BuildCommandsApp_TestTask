using Application;
using ConsoleApplication;
using Domain;
using SimpleSpeedTests;
using System.Collections.Concurrent;

string bigFileName = "bigFile.txt";

TargetCreator fileBuilder = new TargetCreator(bigFileName);
fileBuilder.CountOfTargets = 1000000;
fileBuilder.MinDependentTarget = 0;
fileBuilder.MaxDependentTarget = 3;
fileBuilder.MinActionsInTarget = 1;
fileBuilder.MaxActionsInTarget = 4;
fileBuilder.CreateFile();


SimpleSpeedTest test = new SimpleSpeedTest(bigFileName);
test.RunTests();
test.PrintReport();

string simpleFile = "makeFile.txt";

// Получаем имя файла
var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bigFileName);

// Получаем массив строк из файла
var lines = File.ReadAllLines(settingsPath);

// Парсим строки в словарь
ConcurrentDictionary<string, Target> targetsDictionary = new();
try
{
    ParseMakeFile parseMakeFile = new ParseMakeFile(lines);
    targetsDictionary = parseMakeFile.GetConcurrentDictionaryParallel();
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Ошибка выполнения: {ex.Message}");
}

ConsoleApp consoleApplication = new ConsoleApp(targetsDictionary);
consoleApplication.Run();


fileBuilder.DeleteFile();
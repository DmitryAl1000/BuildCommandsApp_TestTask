using Application;
using ConsoleApplication;
using Domain;
using SimpleSpeedTests;
using System.Collections.Concurrent;


//Генерируем файл если нужно
bool generateFile = false;
string generatedFileName = "generatedFile.txt";
TargetCreator fileBuilder = new TargetCreator(generatedFileName);
fileBuilder.CountOfTargets = 1000000;
fileBuilder.MinDependentTarget = 0;
fileBuilder.MaxDependentTarget = 3;
fileBuilder.MinActionsInTarget = 1;
fileBuilder.MaxActionsInTarget = 4;

if (generateFile)
{
    fileBuilder.CreateFile();
}

//Имеющиеся файлы
string bigFileName = "bigFile.txt"; //1 мил строк. сгенерированный 
string simpleFile = "makeFile.txt"; //cо всякими ошибками, циклами и прочим

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

if (generateFile)
{
    fileBuilder.DeleteFile();
}
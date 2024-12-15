using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Application
{
    // Не используется. Для создания файла с задачами, для тестов
    public class TargetCreator
    {
        const string TARGET_NAME = "Target";

        public int CountOfTargets { get; set; } = 10;

        public int MaxDependentTarget { get; set; } = 2;

        private int _minDependentTarget = 0;
        public int MinDependentTarget
        {
            get { return _minDependentTarget; }
            set
            {
                if (value < 0)
                {
                    _minDependentTarget = 0;
                }
                _minDependentTarget = value;
            }
        }

        public int MaxActionsInTarget { get; set; } = 5;

        private int _minActionsInTarget = 0;
        public int MinActionsInTarget
        {
            get { return _minActionsInTarget; }
            set
            {
                if (value < 0)
                {
                    _minActionsInTarget = 0;
                }
                _minActionsInTarget = value;
            }
        }

        private string FileName { get; set; }

        private string[] actionsNames = new string[] { "sort", "get", "set", "delete", "create", "update", "send", "start", "run", "getFlowers", "eat", "sleep", };



        public TargetCreator(string fileName)
        {
            this.FileName = fileName;
        }

        public void CreateFile()
        {
            string text = GenerateText();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);

            using (FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                byte[] input = Encoding.Default.GetBytes(text);
                fstream.Write(input, 0, input.Length);
            }

        }

        public void DeleteFile()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            FileInfo fileInf = new FileInfo(filePath);
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
        }

        // Создаем большую строку по заданным параметрам
        private string GenerateText()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int x = 0; x < CountOfTargets; x++)
            {
                Random rnd = new Random();
                int dependentTargets = rnd.Next(MinDependentTarget, MaxDependentTarget);
                int actionsIntarget = rnd.Next(MinActionsInTarget, MaxActionsInTarget);

                stringBuilder.Append(TARGET_NAME + $"{x}:");
                for (int y = 0; y < dependentTargets; y++)
                {
                    int targetNum = rnd.Next(0, CountOfTargets - 1); //-1 потому как нумерация идет с нуля.

                    stringBuilder.Append(" " + TARGET_NAME + $"{targetNum}");
                }
                stringBuilder.Append("\n");

                for (int z = 0; z < actionsIntarget; z++)
                {
                    int actionNum = rnd.Next(0, actionsNames.Length);
                    stringBuilder.Append(" " + actionsNames[actionNum] + $"\n");
                }
            }

            return stringBuilder.ToString();
        }




    }
}

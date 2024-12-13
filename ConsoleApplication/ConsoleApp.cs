using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotePadeContext;

namespace ConsoleApplication
{
    public class ConsoleApp
    {
        public void Run()
        {
            var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "makeFile.txt");
            var task = RaeadFromNotePade.ReadFromNotePade(settingsPath);
            var stringAllFile = task.Result;

            Console.WriteLine(stringAllFile);
        }

        public void 







    }
}

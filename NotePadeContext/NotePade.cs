using System.IO;
using System.Text;
using Domain;

namespace NotePadeContext
{
    public static class NotePade
    {
        public static async Task<string[]> GetLinesAsync(string path)
        {
            var lines =  File.ReadAllLinesAsync(path);
            await lines;
            return lines.Result;
        }
        public static string[] ReadAllLines(string path)
        {
            var lines = File.ReadAllLines(path);
            return lines;
        }

        public static string GetText(string path)
        {
            var lines = File.ReadAllText(path);
            return lines;
        }

    }
}

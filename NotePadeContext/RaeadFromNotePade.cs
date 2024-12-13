using System.IO;
using System.Text;

namespace NotePadeContext
{
    public static class RaeadFromNotePade
    {

        public static async Task<string> ReadFromNotePade(string path)
        {
            
            using (FileStream fstream = File.OpenRead(path))
            {
                // выделяем массив для считывания данных из файла
                byte[] buffer = new byte[fstream.Length];
                // считываем данные
                await fstream.ReadAsync(buffer, 0, buffer.Length);

                return Encoding.Default.GetString(buffer);
            }


        }





    }
}

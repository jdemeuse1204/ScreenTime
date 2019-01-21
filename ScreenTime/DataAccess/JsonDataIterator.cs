using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace ScreenTime.DataAccess
{
    public class JsonDataIterator<T> : IEnumerable<T> where T : class
    {
        private readonly string FileNameAndPath;

        public JsonDataIterator(string fileNameAndPath)
        {
            FileNameAndPath = fileNameAndPath;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            string line;

            // Read the file and display it line by line.  
            using (var file = new System.IO.StreamReader(FileNameAndPath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    yield return JsonConvert.DeserializeObject<T>(line);
                }
            }
        }
    }
}

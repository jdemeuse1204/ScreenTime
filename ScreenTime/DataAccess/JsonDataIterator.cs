using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ScreenTime.DataAccess
{
    public class JsonDataIterator<T> : IEnumerable<T>, IDisposable where T : class
    {
        private readonly string FileNameAndPath;
        private readonly StreamReader StreamReader;

        public JsonDataIterator(string fileNameAndPath)
        {
            FileNameAndPath = fileNameAndPath;
            StreamReader = new StreamReader(FileNameAndPath);
        }

        public void Dispose()
        {
            StreamReader.Dispose();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            string line;

            while ((line = StreamReader.ReadLine()) != null)
            {
                yield return JsonConvert.DeserializeObject<T>(line);
            }
        }
    }
}

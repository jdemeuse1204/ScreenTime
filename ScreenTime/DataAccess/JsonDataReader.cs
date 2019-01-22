using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScreenTime.DataAccess
{
    public class JsonDataReader
    {
        private readonly string FileNameAndPath;
        public JsonDataReader(string filePath, string fileName)
        {
            FileNameAndPath = Path.Combine(filePath, fileName);

            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
        }

        public T Get<T>(Func<T, bool> expression) where T : class
        {
            if (File.Exists(FileNameAndPath) == false)
            {
                return default(T);
            }

            using (var iterator = new JsonDataIterator<T>(FileNameAndPath))
            {
                return iterator.FirstOrDefault(expression);
            }
        }

        public IEnumerable<T> All<T>(Func<T, bool> expression) where T : class
        {
            if (File.Exists(FileNameAndPath) == false)
            {
                return new List<T>();
            }

            using (var iterator = new JsonDataIterator<T>(FileNameAndPath))
            {
                return iterator.Where(expression);
            }
        }

        public IEnumerable<T> All<T>() where T : class
        {
            if (File.Exists(FileNameAndPath) == false)
            {
                return new List<T>();
            }

            using (var iterator = new JsonDataIterator<T>(FileNameAndPath))
            {
                return iterator.ToList();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenTime.DataAccess
{
    public class JsonDataReader
    {
        private readonly string FileNameAndPath;
        public JsonDataReader(string fileNameAndPath)
        {
            FileNameAndPath = fileNameAndPath;
        }

        public T Get<T>(Func<T, bool> expression) where T : class
        {
            return new JsonDataIterator<T>(FileNameAndPath).FirstOrDefault(expression);
        }

        public IEnumerable<T> All<T>(Func<T, bool> expression) where T : class
        {
            return new JsonDataIterator<T>(FileNameAndPath).Where(expression);
        }

        public IEnumerable<T> All<T>() where T : class
        {
            return new JsonDataIterator<T>(FileNameAndPath);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScreenTime.DataAccess
{
    public class JsonDataWriter
    {
        private readonly string FileNameAndPath;
        public JsonDataWriter(string filePath, string fileName)
        {
            FileNameAndPath = Path.Combine(filePath, fileName);

            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
        }

        public void Remove<T>(Func<T, bool> expression) where T : class
        {
            using (var iterator = new JsonDataIterator<T>(FileNameAndPath))
            {
                var itemsToSave = iterator.Where(w => expression(w) == false);

                Add(itemsToSave);
            }
        }

        public void Add<T>(IEnumerable<T> items) where T : class
        {
            File.WriteAllLines(FileNameAndPath, items.Select(JsonConvert.SerializeObject));
        }
    }
}

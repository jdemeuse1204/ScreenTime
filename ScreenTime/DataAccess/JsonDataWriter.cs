using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTime.DataAccess
{
    public class JsonDataWriter
    {
        private readonly string FileNameAndPath;
        
        public JsonDataWriter(string fileNameAndPath)
        {
            FileNameAndPath = fileNameAndPath;
        }

        public bool Remove<T>(Func<T, bool> expression) where T : class
        {
            var itemsToSave = new JsonDataIterator<T>(FileNameAndPath).Where(w => expression(w) == false);


        }
    }
}

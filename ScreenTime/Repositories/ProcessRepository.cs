using ScreenTime.DataAccess;
using ScreenTime.DataAccess.Registry;
using ScreenTime.Types;
using System.Collections.Generic;

namespace ScreenTime.Repositories
{
    public class ProcessRepository
    {
        private readonly JsonDataWriter JsonDataWriter = new JsonDataWriter(ApplicationSettings.LogFileLocation, ApplicationSettings.LogFileName);
        private readonly JsonDataReader JsonDataReader = new JsonDataReader(ApplicationSettings.LogFileLocation, ApplicationSettings.LogFileName);

        public void Save(IEnumerable<WindowsProcess> processes)
        {
            JsonDataWriter.Add(processes);
        }

        public IEnumerable<WindowsProcess> LoadSavedProcesses()
        {
            return JsonDataReader.All<WindowsProcess>();
        }
    }
}

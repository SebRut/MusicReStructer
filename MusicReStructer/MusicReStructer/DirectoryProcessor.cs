using System.IO;
using System.Linq;
using System.Threading;

namespace MusicReStructer
{
    public class DirectoryProcessor
    {
        private readonly bool recursive;
        private readonly DirectoryConfiguration configuration;
        private readonly string path;
        private ManualResetEvent[] directoryEvents;
        private ManualResetEvent[] fileEvents;

        public DirectoryProcessor(string directory, bool recursive, DirectoryConfiguration configuration)
        {
            path = directory;
            this.recursive = recursive;
            this.configuration = configuration;
        }

        /// <summary>
        /// Processes the directory
        /// </summary>
        /// <param name="state">data for status response</param>
        public void ProcessDirectory(object state)
        {
            var matchingFiles = from file in Directory.GetFiles(path) where new FileInfo(file).Extension == ".mp3" select file;

            fileEvents = new ManualResetEvent[matchingFiles.Count()];

            for(int i = 0; i < matchingFiles.ToArray().Length; i++)
            {
                string file = matchingFiles.ToArray()[i];
                fileEvents[i] = new ManualResetEvent(false);
                var processor = new FileProcessor(file, configuration);
                ThreadPool.QueueUserWorkItem(processor.ProcessFile, new object[]{i, fileEvents});
            }

            if (recursive)
            {            
                var directories = Directory.GetDirectories(path).Where(dir => dir != configuration.TempDirectory);

                directoryEvents = new ManualResetEvent[directories.ToArray().Length];

                for (int i = 0; i < directories.ToArray().Length; i++)
                {
                    var directory = directories.ToArray()[i];
                    directoryEvents[i] = new ManualResetEvent(false);
                    var processor = new DirectoryProcessor(directory, recursive, configuration);
                    ThreadPool.QueueUserWorkItem(processor.ProcessDirectory, new object[]{i, directoryEvents});
                }
            }

            if(fileEvents.Length > 0 || directoryEvents.Length > 0) WaitHandle.WaitAll(directoryEvents.Concat(fileEvents).ToArray());

            ManualResetEvent[] callBackArray = ((object[]) state)[1] as ManualResetEvent[];
            callBackArray[(int) ((object[])state)[0]].Set();
            System.Diagnostics.Debug.WriteLine(path + " finished");
        }
    }
}

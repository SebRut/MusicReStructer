using System.IO;
using System.Linq;
using System.Threading;


namespace MusicReStructer
{
    /// <summary>
    /// 
    /// </summary>
    public class MusicDirectory
    {
        /// <summary>
        /// The configuration for that directory
        /// </summary>
        public DirectoryConfiguration Configuration;

        private bool recursive;

        /// <summary>
        /// Initates a new MusicDirectory or laods an existing configuration
        /// </summary>
        /// <param name="directory">the base directory</param>
        /// <param name="recursive">indicates whether subdirs should get searched aswell</param>
        public MusicDirectory(string directory, bool recursive)
        {
            this.recursive = recursive;
            Configuration = new DirectoryConfiguration(directory);

            if (File.Exists(Configuration.ConfigFile))
            {
                Configuration = DirectoryConfiguration.LoadConfiguration(Configuration.ConfigFile);
            }
        }

        /// <summary>
        /// starts the processing of the directory
        /// </summary>
        public void StartProcessing()
        {
            if (!Directory.Exists(Configuration.TempDirectory)) Directory.CreateDirectory(Configuration.TempDirectory);
            if (!Directory.Exists(Configuration.WorkDirectory)) Directory.CreateDirectory(Configuration.WorkDirectory);

            var directories = from dir in Directory.GetDirectories(Configuration.BaseDirectory)
                where dir != Configuration.TempDirectory
                select dir;

            foreach (var directory in directories)
            {
             Directory.Move(directory, Path.Combine(Configuration.WorkDirectory, new DirectoryInfo(directory).Name));   
            }

            foreach (var file in Directory.GetFiles(Configuration.BaseDirectory))
            {
                File.Move(file, Path.Combine(Configuration.WorkDirectory, new FileInfo(file).Name));
            }

            ManualResetEvent[] tempArray = new ManualResetEvent[]{new ManualResetEvent(false)};
            ThreadPool.QueueUserWorkItem(new DirectoryProcessor(Configuration.WorkDirectory, recursive, Configuration).ProcessDirectory, new object[]{0, tempArray});
            WaitHandle.WaitAll(tempArray);
            System.Diagnostics.Debug.WriteLine("Processing finished");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Id3Lib;

namespace MusicReStructer
{
    public class MusicDirectory
    {
        private DirectoryConfiguration configuration;
    
        public MusicDirectory(string directory)
        {
            configuration = new DirectoryConfiguration(directory);

            if (File.Exists(configuration.ConfigFile))
            {
                configuration = DirectoryConfiguration.LoadConfiguration(configuration.ConfigFile);
            }
        }

        public void StartProcessing()
        {
            if (!Directory.Exists(configuration.TempDirectory)) Directory.CreateDirectory(configuration.TempDirectory);
            if (!Directory.Exists(configuration.WorkDirectory)) Directory.CreateDirectory(configuration.WorkDirectory);

            var directories = from dir in Directory.GetDirectories(configuration.BaseDirectory)
                where dir != configuration.TempDirectory
                select dir;

            foreach (var directory in directories)
            {
             Directory.Move(directory, Path.Combine(configuration.WorkDirectory, new DirectoryInfo(directory).Name));   
            }

            foreach (var file in Directory.GetFiles(configuration.BaseDirectory))
            {
                File.Move(file, Path.Combine(configuration.WorkDirectory, new FileInfo(file).Name));
            }

            ThreadPool.QueueUserWorkItem(new DirectoryProcessor(configuration.WorkDirectory, true, configuration).ProcessDirectory);
        }
    }
}

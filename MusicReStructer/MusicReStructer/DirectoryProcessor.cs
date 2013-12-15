using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace MusicReStructer
{
    public class DirectoryProcessor
    {
        private readonly bool recursive;
        private readonly DirectoryConfiguration configuration;
        private readonly string path;

        public DirectoryProcessor(string directory, bool recursive, DirectoryConfiguration configuration)
        {
            path = directory;
            this.recursive = recursive;
            this.configuration = configuration;
        }

        public void ProcessDirectory(object state)
        {
            var matchingFiles = from file in Directory.GetFiles(path) where new FileInfo(file).Extension == ".mp3" select file;

            foreach (string file in matchingFiles)
            {
                var processor = new FileProcessor(file, configuration);
                ThreadPool.QueueUserWorkItem(processor.ProcessFile);
            }

            if (recursive)
            {
                List<string> directories = new List<string>(Directory.GetDirectories(path));

                foreach (var directory in directories)
                {
                    var processor = new DirectoryProcessor(directory, recursive, configuration);
                    ThreadPool.QueueUserWorkItem(processor.ProcessDirectory);
                }
            }
        }
    }
}

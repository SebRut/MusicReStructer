using System.IO;
using TFile = TagLib.File;

namespace MusicReStructer
{
    public class FileProcessor
    {
        private readonly string file;
        private readonly DirectoryConfiguration configuration;

        public FileProcessor(string file, DirectoryConfiguration configuration)
        {
            this.file = file;
            this.configuration = configuration;
        }

        public void ProcessFile(object state)
        {
            //ID3v1 tagSet = new ID3v1();
            //tagSet.Deserialize(File.OpenRead(file));

            TFile tagFile = TFile.Create(file);

            AudioPathSet pathSet = new AudioPathSet(tagFile, configuration, new FileInfo(file).Extension);
            pathSet.CheckDirs();

            File.Move(file, pathSet.TrackPath);

        }
    }
}

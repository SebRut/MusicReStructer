using System.IO;
using System.Threading;
using TFile = TagLib.File;

namespace MusicReStructer
{
    /// <summary>
    /// A class for async music file processing
    /// </summary>
    public class FileProcessor
    {
        private readonly string file;
        private readonly DirectoryConfiguration configuration;

        /// <summary>
        /// Initiates a new FileProcessor
        /// </summary>
        /// <param name="file">the fiel to process</param>
        /// <param name="configuration">the config to read from</param>
        public FileProcessor(string file, DirectoryConfiguration configuration)
        {
            this.file = file;
            this.configuration = configuration;
        }

        /// <summary>
        /// Processes the audio file
        /// </summary>
        /// <param name="state">the parameters for status response</param>
        public void ProcessFile(object state)
        {

            TFile tagFile = TFile.Create(file);

            AudioPathSet pathSet = new AudioPathSet(tagFile, configuration, new FileInfo(file).Extension);
            pathSet.CheckDirs();

            File.Move(file, pathSet.TrackPath);

            ManualResetEvent[] callBackArray = ((object[])state)[1] as ManualResetEvent[];
            callBackArray[(int)((object[])state)[0]].Set();

            System.Diagnostics.Debug.WriteLine(file + " finished");
        }
    }
}

using System;
using System.IO;
using System.Xml.Serialization;

namespace MusicReStructer
{
    public struct DirectoryConfiguration
    {
        public const string WorkDirectoryName = "files";
        public const string ConfigFileName = "config.xml";
        public const string TempDirectoryName = ".musicrestructer";
        public readonly string BaseDirectory;

        public DirectoryConfiguration(string directory)
        {
            BaseDirectory = directory;
        }

        public string WorkDirectory
        {
            get { return Path.Combine(TempDirectory, WorkDirectoryName); }
        }

        public string TempDirectory
        {
            get { return Path.Combine(BaseDirectory, TempDirectoryName); }
        }

        public string ConfigFile
        {
            get { return Path.Combine(TempDirectory, ConfigFileName); }
        }

        /// <summary>
        /// Loads a Directory Configuration from the given configfile
        /// </summary>
        /// <param name="file">The config file to load</param>
        /// <returns>a desrialized DirectoryConfiguration</returns>
        public static DirectoryConfiguration LoadConfiguration( string file)
        {
            DirectoryConfiguration configuration;
            var xmlSerializer = new XmlSerializer(typeof (DirectoryConfiguration));

            using (FileStream stream = File.OpenRead(file))
            {
                configuration = (DirectoryConfiguration) xmlSerializer.Deserialize(stream);
                stream.Close();
            }

            return configuration;
        }

        /// <summary>
        /// Saves the object(Configuration) into its configfile
        /// </summary>
        public void SaveConfiguration()
        {
            var xmlSerializer = new XmlSerializer(typeof(DirectoryConfiguration));

            using (FileStream stream = File.OpenWrite(ConfigFile))
            {
                xmlSerializer.Serialize(stream, this);
                stream.Close();
            }
        }
    }
}
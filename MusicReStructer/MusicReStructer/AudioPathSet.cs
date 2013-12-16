using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using File = System.IO.File;
using TFile = TagLib.File;


namespace MusicReStructer
{
    public class AudioPathSet
    {
        private readonly TFile tagFile;
        private readonly DirectoryConfiguration configuration;
        private readonly string extension;

        public AudioPathSet(TFile tagFile, DirectoryConfiguration configuration, string extension)
        {
            this.tagFile = tagFile;
            this.configuration = configuration;
            this.extension = extension;
        }

        public string ArtistPath
        {
            get
            {
                string ret = String.Empty;
                if (tagFile.Tag.JoinedAlbumArtists != "")ret = Path.Combine(configuration.BaseDirectory,ValidatePath(tagFile.Tag.JoinedAlbumArtists));
                else if (tagFile.Tag.JoinedPerformers != "")
                    ret = Path.Combine(configuration.BaseDirectory, ValidatePath(tagFile.Tag.JoinedPerformers));
                else ret = Path.Combine(configuration.BaseDirectory, ValidatePath("Unknown"));
                return ValidatePath(ret);
            }
        }

        public string AlbumPath
        {
            get
            {
                string ret = String.Empty;
                if (tagFile.Tag.Album != null) ret = Path.Combine(ArtistPath, ValidatePath(tagFile.Tag.Album));
                else ret = Path.Combine(ArtistPath, "Unknown");
                return ValidatePath(ret);
            }
        }

        private string ValidatePath(string path)
        {
            string regexSearch = string.Format("{0}{1}", new string(Path.GetInvalidPathChars()), '*');
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        public string TrackPath
        {
            get
            {
                string file = String.Empty;
                if (tagFile.Tag.Title != null) file = ValidateFile(tagFile.Tag.Title);
                else file = "Track ";

                string j = "";
                while (File.Exists(Path.Combine(AlbumPath, file + j + extension)))
                {
                    if (j == "") j = "0";
                    j = (Convert.ToInt32(j) + 1).ToString();
                }
                return Path.Combine(AlbumPath, file+j + extension);
            }
        }

        private string ValidateFile(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) +new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        public void CheckDirs()
        {
            if (!Directory.Exists(ArtistPath)) Directory.CreateDirectory(ArtistPath);
            if (!Directory.Exists(AlbumPath)) Directory.CreateDirectory(AlbumPath);
        }
    }
}

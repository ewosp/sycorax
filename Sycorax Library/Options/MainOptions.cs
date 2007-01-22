/*
 * (c) Sébastien Santoro aka Dereckson, 2006, tous droits réservés
 *
 * Date: 13/07/2006
 * Time: 21:10
 *
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Sycorax {

    #region enum Id3Version, ReadId3, WriteId3v1, WriteId3v2

    /// <summary>
    /// Gestion des tags id3 en  lecture
    /// </summary>
    public enum ReadId3 {
        //Ne pas lire les tags id3
        No = 0,
        //en tant que source primaire (overwrite path datas)
        AsPrimarySource = 1,
        //en tant que source secondaire (complete path datas)
        AsAlternativeSource = 2,
    }

    /// <summary>
    /// Gestion des tags id3v1 en  écriture
    /// </summary>
    public enum WriteId3v1 {
        //Ne pas écrire 
        No = 0,
        //Les compléter
        Complete = 1,
        //Les écraser
        Overwrite = 2
    }

    /// <summary>
    /// Gestion des tags id3v2 en  écriture
    /// </summary>
    public enum WriteId3v2 {
        //Ne pas écrire 
        No = 0,
        //Les compléter
        Complete = 1,
        //Les écraser
        Overwrite = 2
    }

    /// <summary>
    /// Tag id3 prioritaire
    /// </summary>
    public enum Id3Version {
        v1 = 1,
        v2 = 2
    }
    #endregion

    /// <summary>
    /// Main Options.
    /// </summary>
    [Serializable()]
    public class MainOptions {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MainOptions"/> class.
        /// </summary>
        public MainOptions () {
            //Version 0.1 des préférences
            Version = new Version(0, 1);
        }

        /// <summary>
        /// Sets the defaults options.
        /// </summary>
        public void SetDefaults () {
            connectionString = "Server=localhost;Database=Sycorax;Uid=root;Pwd=;";
            debugMode = true;
            deleteTunesIfOrphans = false;
            extensionsToWatch = new List<string>(new string[14] {
                //Audio files
                ".mp3", ".aac", ".ogg", ".wav", ".wma", ".mpc", ".mp4", ".flac", ".mid", ".m4a",
                //Video files
                ".wmv", ".mpg", ".mpeg", ".avi"
            });
            foldersToWatch = new List<string>();
            mainStorageServer = "mysql";
            priorId3Tag = Id3Version.v2;
            readId3Tags = ReadId3.No;
        }

        #region Sérialisation
        /// <summary>
        /// Loads the default options file.
        /// </summary>
        /// <returns>Instance of this options class based on options files</returns>
        public static MainOptions Load () {
            return Load(DefaultOptionsFile);
        }

        /// <summary>
        /// Loads the specified options file.
        /// </summary>
        /// <param name="optionsFile">The options file.</param>
        /// <returns>Instance of this options class based on options files</returns>
        static public MainOptions Load (string optionsFile) {
            if (!File.Exists(optionsFile)) {
                return CreateNewOptionsFile(optionsFile);
            }

            // Create an XmlSerializer to use for retrieving options values
            XmlSerializer mySerializer = new XmlSerializer(typeof(MainOptions));

            // Create a StreamReader to point to the options file
            StreamReader myTextReader = new StreamReader(optionsFile);

            // Create an XmlTextReader to actually read the options.
            XmlTextReader myXmlReader = new XmlTextReader(myTextReader);

            if (mySerializer.CanDeserialize(myXmlReader)) {
                MainOptions options = ((MainOptions)mySerializer.Deserialize(myXmlReader));
                // Close the IO objects we've used.
                myXmlReader.Close();
                myTextReader.Close();
                // Return deserialized result
                return options;
            } else {
                // Close the IO objects we've used.
                myXmlReader.Close();
                myTextReader.Close();
                return CreateNewOptionsFile(optionsFile);
            }
        }

        /// <summary>
        /// Saves the specified options file.
        /// </summary>
        public void Save () {
            Save(DefaultOptionsFile);
        }

        /// <summary>
        /// Creates a new options file.
        /// </summary>
        /// <param name="optionsFile">The filename.</param>
        static public MainOptions CreateNewOptionsFile (string optionsFile) {
            //Le dossier existe-t-il ?
            string folder = Path.GetDirectoryName(MainOptions.DefaultOptionsFile);
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            //Créons un nouveau fichier 
            //contenant les préférences par défaut
            //et enregistrons-le
            MainOptions options = new MainOptions();
            options.SetDefaults();
            options.Save(optionsFile);
            return options;
        }

        /// <summary>
        /// Sauvegarde les préférences
        /// </summary>
        /// <param name="filename">fichier où les préférences doivent être sauvegardées</param>
        public void Save (string filename) {
            // Create a stream writer to overwrite any files currently there,
            // so that the fresh options can be saved.
            StreamWriter myWriter = new StreamWriter(filename);
            // Create an XML Serializer to serialize the object
            XmlSerializer myXmlSerializer = new XmlSerializer(this.GetType());
            // Serialize the current Options object (this) to disk.
            myXmlSerializer.Serialize(myWriter, this);
            // Close the writer.
            myWriter.Flush();
            myWriter.Close();
        }
        #endregion

        private bool debugMode;
        /// <summary>
        /// Gets or sets a value indicating whether debug mode is enabled.
        /// </summary>
        /// <value><c>true</c> if debug mode is enabled; otherwise, <c>false</c>.</value>
        public bool DebugMode {
            get { return debugMode; }
            set { debugMode = value; }
        }

        private bool deleteTunesIfOrphans;
        /// <summary>
        /// Gets or sets a value indicating whether tunes has to be deleted if orphans.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if we've to delete orphans tunes ; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteTunesIfOrphans {
            get { return deleteTunesIfOrphans; }
            set { deleteTunesIfOrphans = value; }
        }

        

        #region Databases stuff
        /// <summary>
        /// Storage Engine
        /// </summary>
        /// <remarks>Not an enum to be more evolutive</remarks>
        private string mainStorageServer;
        public string  MainStorageServer {
            get {
                return mainStorageServer;
            }
            set {
                mainStorageServer = value;
            }
        }

        private string connectionString;
        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        /// <value>The database connection string.</value>
        public string ConnectionString {
            get {
                return connectionString;
            }
            set {
                connectionString = value;
            }
        }
        #endregion

        #region Propriétés Id3
        private ReadId3 readId3Tags;
        /// <summary>
        /// Ecrire les tag id3v2 ?
        /// </summary>
        public ReadId3 ReadId3Tags {
            get {
                return readId3Tags;
            }
            set {
                readId3Tags = value;
            }
        }


        private WriteId3v1 writeId3v1Tags;
        /// <summary>
        /// Ecrire les tag id3v2 ?
        /// </summary>
        public WriteId3v1 WriteId3v1Tags {
            get {
                return writeId3v1Tags;
            }
            set {
                writeId3v1Tags = value;
            }
        }


        private WriteId3v2 writeId3v2Tags;
        /// <summary>
        /// Ecrire les tag id3v2 ?
        /// </summary>
        public WriteId3v2 WriteId3v2Tags {
            get {
                return writeId3v2Tags;
            }
            set {
                writeId3v2Tags = value;
            }
        }

        private Id3Version priorId3Tag;
        /// <summary>
        /// Lequel des deux tags a la priorité ?
        /// </summary>
        public Id3Version PriorId3Tag {
            get {
                return priorId3Tag;
            }
            set {
                priorId3Tag = value;
            }
        }
        #endregion

        #region Listes des dossiers sous surveillance (FoldersToWatch, AddFolder, DelFolder)
        private List<string> foldersToWatch;
        /// <summary>
        /// Gets or sets the folders to watch.
        /// </summary>
        /// <value>The folders to watch.</value>
        public string[] FoldersToWatch {
            get {
                return foldersToWatch.ToArray();
            }
            set {
                foldersToWatch = new List<string>(value);
            }
        }

        
        /// <summary>
        /// Adds a folder to the watch list.
        /// </summary>
        /// <param name="path">The folder path.</param>
        public void AddFolder (string path) {
            if (!foldersToWatch.Contains(path)) {
                foldersToWatch.Add(path);
            }
        }

        /// <summary>
        /// Dels the folder from the watch list
        /// </summary>
        /// <param name="path">The folder path.</param>
        public void DelFolder (string path) {
            if (foldersToWatch.Contains(path)) {
                foldersToWatch.Remove(path);
            }
        }
        #endregion

        #region Extensions to watch
        private List<string> extensionsToWatch;
        /// <summary>
        /// Gets or sets the extensions to watch.
        /// </summary>
        /// <value>The extensions to watch.</value>
        public string[] ExtensionsToWatch {
            get {
                return extensionsToWatch.ToArray();
            }
            set {
                extensionsToWatch = new List<string>(value);
            }
        }

        /// <summary>
        /// Determines whether the specified extension is watched.
        /// </summary>
        /// <param name="extension">The extension to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified extension is watched ; otherwise, <c>false</c>.
        /// </returns>
        public bool isExtensionWatched (string extension) {
            try {
                return extensionsToWatch.Contains(extension);
            } catch {
                return false;
            }           
        }

        /// <summary>
        /// Adds a folder to the watch list.
        /// </summary>
        /// <param name="path">The folder path.</param>
        public void AddExtension (string path) {
            if (!foldersToWatch.Contains(path)) {
                extensionsToWatch.Add(path);
            }
        }

        /// <summary>
        /// Dels the folder from the watch list
        /// </summary>
        /// <param name="path">The folder path.</param>
        public void DelExtension (string path) {
            if (extensionsToWatch.Contains(path)) {
                extensionsToWatch.Remove(path);
            }
        }
        #endregion
 
        /// <summary>
        /// Gets the default options file.
        /// </summary>
        /// <value>The default options file.</value>
        static public string DefaultOptionsFile {
            get {
                return Path.Combine(
                    //document and settings > <user profile> > local settings > application data > Sycorax
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    @"Sycorax\SycoraxAutoUpdate.config"
                );
            }
        }

        /// <summary>
        /// Version des options
        /// (utilisé pour compléter la déséralisation par les nouvelles valeurs par défaut)
        /// </summary>
        private Version Version;
    }
}
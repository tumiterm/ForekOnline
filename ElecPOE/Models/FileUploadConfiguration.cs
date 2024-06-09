namespace ElecPOE.Models
{
    public class FileUploadConfiguration
    {
        public string SourceFile { get; }
        public string DestinationFolder { get; }
        public string DestinationFileName { get; }

        public FileUploadConfiguration(string sourceFile, string destinationFolder, string destinationFileName)
        {
            SourceFile = sourceFile;
            DestinationFolder = destinationFolder;
            DestinationFileName = destinationFileName;
        }
    }


}

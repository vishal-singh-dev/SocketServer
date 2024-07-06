namespace SocketServer.Services
{
    public class FolderService
    {
        public string FolderPath { get; }

        public FolderService(string folderPath)
        {
            FolderPath = folderPath;
        }
    }
}

namespace VkWallPostgre.Infostracture
{
    public class FileLogger : ISimpleLogger
    {
        private readonly string fileName;
        public FileLogger(string fileName)
        {
            this.fileName = fileName;
        }
        public void Log(string action)
        {
            File.AppendAllText(fileName, $"[{DateTime.Now}] {action}\n");
        }
    }
}

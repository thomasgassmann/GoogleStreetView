namespace TG.Tools
{
    using System.IO;

    public static class FileStatus
    {
        public static bool IsFileLocked(string path)
        {
            var file = new FileInfo(path);
            var stream = default(FileStream);
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }
    }
}

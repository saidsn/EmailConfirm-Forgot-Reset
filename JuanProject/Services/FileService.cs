using JuanProject.Services.Interfaces;

namespace JuanProject.Services
{
    public class FileService : IFileService
    {
        public string ReadFile(string path, string readTemplate)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                readTemplate = reader.ReadToEnd();
            }

            return readTemplate;
        }
    }
}

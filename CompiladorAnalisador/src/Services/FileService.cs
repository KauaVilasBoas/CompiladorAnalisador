using CompiladorAnalisador.Interfaces;

namespace CompiladorAnalisador.Services
{
    public class FileService : IFileService
    {
        public StreamReader OpenFile(string path)
        {
            ValidateFile(path);
            var fullPath = GetFullPath(path);
            return new StreamReader(fullPath);
        }

        public void ValidateFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Caminho do arquivo é obrigatório");

            var fullPath = GetFullPath(path);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Arquivo não encontrado: {fullPath}");
        }

        public string GetOutputPath(string inputPath, string extension)
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));

            var outputDirectory = Path.Combine(projectRoot, "Output", "Report");

            Directory.CreateDirectory(outputDirectory);

            var baseFileName = Path.GetFileNameWithoutExtension(inputPath);

            return Path.Combine(outputDirectory, baseFileName + extension);
        }

        private string GetFullPath(string path)
        {
            return Path.IsPathRooted(path)
                ? path
                : Path.Combine(Directory.GetCurrentDirectory(), path);
        }
    }
}
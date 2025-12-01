namespace CompiladorAnalisador.Interfaces;

public interface IFileService
{
    StreamReader OpenFile(string path);
    void ValidateFile(string path);
    string GetOutputPath(string inputPath, string extension);
}
using CompiladorAnalisador.Models;

namespace CompiladorAnalisador.Interfaces;

public interface ILexicalAnalyzer
{
    List<Token> AnalyzeFile(string filePath);
}
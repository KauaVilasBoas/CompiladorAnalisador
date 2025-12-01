using System.Text;
using CompiladorAnalisador.Models;

namespace CompiladorAnalisador.Services
{
    public class ReportGenerator
    {
        private readonly FileService _fileService;
        private readonly ReportHeader _reportHeader;

        public ReportGenerator()
        {
            _fileService = new FileService();
            _reportHeader = new ReportHeader();
        }

        public void GenerateLexicalReport(List<Token> tokens, string inputPath)
        {
            var outputPath = _fileService.GetOutputPath(inputPath, ".LEX");

            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                _reportHeader.WriteLexicalHeader(writer);
                WriteTokensTable(writer, tokens);
            }

            Console.WriteLine($"Relatório lexical gerado: {outputPath}");
        }

        public void GenerateSymbolTableReport(List<Token> tokens, string inputPath)
        {
            var outputPath = _fileService.GetOutputPath(inputPath, ".TAB");

            var categoryCTokens = tokens
                .Where(t => t.Code.StartsWith("C") || t.Code == "A22" ||
                            t.Code == "A23")
                .ToList();

            var uniqueSymbols = GroupUniqueSymbols(categoryCTokens);

            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                _reportHeader.WriteSymbolTableHeader(writer);
                WriteSymbolTable(writer, uniqueSymbols);
            }

            Console.WriteLine($"Tabela de símbolos gerada: {outputPath}");
        }

        private void WriteTokensTable(StreamWriter writer, List<Token> tokens)
        {
            writer.WriteLine("{0,-6} | {1,-6} | {2,-35}", "LINHA", "CÓD.", "LEXEMA");
            writer.WriteLine(new string('-', 60));

            foreach (var token in tokens)
            {
                writer.WriteLine("{0,-6} | {1,-6} | {2}",
                    token.Lines.First(), 
                    token.Code,
                    token.Lexeme);
            }
        }

        private void WriteSymbolTable(StreamWriter writer, List<SymbolTableEntry> entries)
        {
            writer.WriteLine("{0,-5} | {1,-5} | {2,-35} | {3,-4} | {4,-3} | {5,-3} | {6}",
                "ENT.", "CÓD.", "LEXEMA", "TIPO", "QTD", "TRU", "LINHAS");
            writer.WriteLine(new string('-', 100));

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                writer.WriteLine("{0,-5} | {1,-5} | {2,-35} | {3,-4} | {4,-3} | {5,-3} | {6}",
                    i + 1,
                    entry.Code,
                    entry.Lexeme,
                    entry.SymbolType,
                    entry.OriginalSize,
                    entry.TruncatedSize,
                    entry.GetFormattedLines());
            }
        }
        
        private List<SymbolTableEntry> GroupUniqueSymbols(List<Token> tokens)
        {
            var grouped = tokens
                .GroupBy(t => t.Lexeme)
                .Select(g =>
                {
                    var firstOccurrence = g.First();

                    var allLines = g.SelectMany(t => t.Lines).Distinct().OrderBy(x => x).ToList();

                    return new SymbolTableEntry
                    {
                        Lexeme = firstOccurrence.Lexeme,
                        Code = firstOccurrence.Code,
                        SymbolType = firstOccurrence.SymbolType,
                        OriginalSize = firstOccurrence.OriginalSize,
                        TruncatedSize = firstOccurrence.AdjustedSize,
                        Lines = allLines
                    };
                })
                .OrderBy(x => x.Code)
                .ThenBy(x => x.Lexeme)
                .ToList();

            return grouped;
        }

        private class SymbolTableEntry
        {
            public string Lexeme { get; set; }
            public string Code { get; set; }
            public string SymbolType { get; set; }
            public int OriginalSize { get; set; }
            public int TruncatedSize { get; set; }
            public List<int> Lines { get; set; }

            public string GetFormattedLines()
            {
                var top5 = Lines.Take(5);
                return string.Join(", ", top5);
            }
        }
    }

    public class ReportHeader
    {
        private const string TEAM_CODE_SYMBOL = "EQ03";

        private const string MEMBER1 = "Kauã Vilas Boas | kaua.boas@ucsal.edu.br | 71 99609-9664";
        private const string MEMBER2 = "Gabriel Cunha | gabrielsouza.cunha@ucsal.edu.br | 71 98212-3984";
        private const string MEMBER3 = "Diogo Ramos | diogo.ramos@ucsal.edu.br | 71 98686-7971";
        private const string MEMBER4 = "Atila Macedo | atila.macedo@ucsal.edu.br | 71 99251-9722";

        public void WriteLexicalHeader(StreamWriter writer)
        {
            writer.WriteLine("Código da Equipe: {0}", TEAM_CODE_SYMBOL);
            writer.WriteLine("Componentes:");
            writer.WriteLine("  {0}", MEMBER1);
            writer.WriteLine("  {0}", MEMBER2);
            writer.WriteLine("  {0}", MEMBER3);
            writer.WriteLine("  {0}", MEMBER4);
            writer.WriteLine("");
            writer.WriteLine("RELATÓRIO DE ANÁLISE LÉXICA");
            writer.WriteLine(new string('=', 30));
        }

        public void WriteSymbolTableHeader(StreamWriter writer)
        {
            writer.WriteLine("Código da Equipe: {0}", TEAM_CODE_SYMBOL);
            writer.WriteLine("Componentes:");
            writer.WriteLine("  {0}", MEMBER1);
            writer.WriteLine("  {0}", MEMBER2);
            writer.WriteLine("  {0}", MEMBER3);
            writer.WriteLine("  {0}", MEMBER4);
            writer.WriteLine("");
            writer.WriteLine("RELATÓRIO DA TABELA DE SÍMBOLOS");
            writer.WriteLine(new string('=', 30));
        }
    }
}
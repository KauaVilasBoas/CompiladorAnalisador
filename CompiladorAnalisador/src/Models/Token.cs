namespace CompiladorAnalisador.Models
{
    public class Token
    {
        public string Lexeme { get; private set; }
        public string Code { get; private set; }
        public int OriginalSize { get; private set; }
        public int AdjustedSize => Lexeme.Length;
        public string SymbolType { get; set; }

        public List<int> Lines { get; private set; }

        public Token(string lexeme, string code, int line, int originalSize)
        {
            Lexeme = lexeme;
            Code = code;
            OriginalSize = originalSize;
            Lines = new List<int> { line };

            SymbolType = InferTypeFromCode(code);
        }

        public void IncrementAppearance(int line)
        {
            if (!Lines.Contains(line))
            {
                Lines.Add(line);
            }
        }

        public string GetLinesString()
        {
            var top5 = Lines.OrderBy(x => x).Take(5);
            return string.Join(", ", top5);
        }

        /// <summary>
        /// Tenta definir o Tipo do Símbolo com base no Código do Átomo.
        /// Útil para constantes numéricas e literais que já nascem tipados.
        /// </summary>
        private string InferTypeFromCode(string code)
        {
            switch (code)
            {
                case "C04": return "IN";
                case "C05": return "FP";
                case "C06": return "ST";
                case "C07": return "CH";

                case "A22":
                case "A23":
                    return "BL";

                default: return "-";
            }
        }
    }
}
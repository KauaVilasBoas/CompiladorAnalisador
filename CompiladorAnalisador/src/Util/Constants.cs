namespace CompiladorAnalisador.Util
{
    public class Constants
    {
        public static class ReservedWords
        {
            public static readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>
            {
                // Categoria A
                { "program", "A01" }, { "declarations", "A02" }, { "endDeclarations", "A03" },
                { "functions", "A04" }, { "endFunctions", "A05" }, { "void", "A06" },
                { "int", "A07" }, { "real", "A08" }, { "char", "A09" }, { "bool", "A10" },
                { "string", "A11" }, { "if", "A12" }, { "then", "A13" }, { "else", "A14" },
                { "endif", "A15" }, { "while", "A16" }, { "do", "A17" }, { "endwhile", "A18" },
                { "return", "A19" }, { "write", "A20" }, { "read", "A21" }, { "true", "A22" },
                { "false", "A23" },

                // Categoria B (Símbolos)
                { "!=", "B01" }, { "!", "B02" }, { "[", "B03" }, { "]", "B04" },
                { "(", "B05" }, { ")", "B06" }, { "{", "B07" }, { "}", "B08" },
                { ";", "B09" }, { ".", "B10" }, { ",", "B11" }, { "=", "B12" },
                { "==", "B13" }, { "<", "B14" }, { "<=", "B15" }, { ">", "B16" },
                { ">=", "B17" }, { "+", "B18" }, { "-", "B19" }, { "*", "B20" },
                { "/", "B21" }, { "%", "B22" }, { "&", "B23" }, { "|", "B24" }
            };
        }

        // Categoria C (Identificadores e Constantes)
        public static class TokenCategory
        {
            public const string ProgramName = "C01";
            public const string Variable = "C02";
            public const string FunctionName = "C03";
            public const string IntConst = "C04";
            public const string RealConst = "C05";
            public const string StringConst = "C06";
            public const string CharConst = "C07";
        }
    }
}
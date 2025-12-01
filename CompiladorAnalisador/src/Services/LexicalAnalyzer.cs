using System.Text;
using CompiladorAnalisador.Models;
using CompiladorAnalisador.Util;

namespace CompiladorAnalisador.Services
{
    public class LexicalAnalyzer
    {
        private readonly FileService _fileService;
        private StreamReader _reader;
        private int _currentState;
        private int _currentLine;
        private readonly List<Token> _tokens;

        public LexicalAnalyzer()
        {
            _fileService = new FileService();
            _tokens = new List<Token>();
            _currentLine = 1;
        }

        public List<Token> AnalyzeFile(string filePath)
        {
            _reader = _fileService.OpenFile(filePath);
            _tokens.Clear();
            _currentLine = 1;

            try
            {
                while (!_reader.EndOfStream)
                {
                    var token = GetNextToken();
                    if (token != null)
                    {
                        ProcessToken(token);
                    }
                }
            }
            finally
            {
                _reader?.Dispose();
            }

            return _tokens.ToList();
        }

        private Token GetNextToken()
        {
            var currentAtom = new StringBuilder();
            char character;
            _currentState = 0;

            while (true)
            {
                if (_reader.EndOfStream && _currentState == 0) return null;

                switch (_currentState)
                {
                    case 0: // Estado Inicial
                        character = (char)_reader.Peek();

                        if (character == '\uffff') return null;

                        if (IsWhiteSpace(character))
                        {
                            ConsumeChar();
                            if (character == '\n') _currentLine++;
                            break;
                        }
                        
                        if (char.IsLetter(character) || character == '_')
                        {
                            _currentState = 1;
                        }
                        // Números (Começam com Dígito)
                        else if (char.IsDigit(character))
                        {
                            _currentState = 2;
                        }
                        // Símbolos e Delimitadores
                        else
                        {
                            // Mapeamento direto para estados de símbolos
                            switch (character)
                            {
                                case '(': return CreateSingleCharToken("B05"); // (
                                case ')': return CreateSingleCharToken("B06"); // )
                                case '{': return CreateSingleCharToken("B07"); // {
                                case '}': return CreateSingleCharToken("B08"); // }
                                case '[': return CreateSingleCharToken("B03"); // [
                                case ']': return CreateSingleCharToken("B04"); // ]
                                case ';': return CreateSingleCharToken("B09"); // ;
                                case ',': return CreateSingleCharToken("B11"); // ,
                                case '+': return CreateSingleCharToken("B18"); // +
                                case '-': return CreateSingleCharToken("B19"); // -
                                case '*': return CreateSingleCharToken("B20"); // *
                                case '%': return CreateSingleCharToken("B22"); // %
                                case '^': return CreateSingleCharToken("B24"); // (Se houver bitwise, ou pipe |)
                                case '&': return CreateSingleCharToken("B23"); // &
                                case '|': return CreateSingleCharToken("B24"); // |
                                case '.': return CreateSingleCharToken("B10"); // .

                                // Símbolos compostos (precisam verificar o próximo)
                                case '/': _currentState = 3; break; // Divisão ou Comentário
                                case '!': _currentState = 6; break; // ! ou !=
                                case '=': _currentState = 7; break; // = ou ==
                                case '<': _currentState = 8; break; // < ou <=
                                case '>': _currentState = 9; break; // > ou >=
                                case '"': _currentState = 10; break; // String
                                case '\'': _currentState = 12; break; // Char

                                default:
                                    ConsumeChar();
                                    break;
                            }
                        }

                        break;

                    case 1: // Identificador ou Palavra Reservada
                        character = ConsumeChar();
                        currentAtom.Append(character);

                        char nextChar = (char)_reader.Peek();

                        // Continua se for Letra, Dígito ou Underscore
                        if (!char.IsLetterOrDigit(nextChar) && nextChar != '_')
                        {
                            string atomStr = currentAtom.ToString();

                            if (Constants.ReservedWords.Dictionary.ContainsKey(atomStr))
                            {
                                string code = Constants.ReservedWords.Dictionary[atomStr];
                                return CreateToken(atomStr, code);
                            }
                            
                            return CreateToken(atomStr, Constants.TokenCategory.Variable);
                        }

                        break;

                    case 2: // Números Inteiros (ou início de Real)
                        character = ConsumeChar();
                        currentAtom.Append(character);

                        char nextForNum = (char)_reader.Peek();

                        if (nextForNum == '.')
                        {
                            _currentState = 4;
                        }
                        else if (!char.IsDigit(nextForNum))
                        {
                            return CreateToken(currentAtom.ToString(), Constants.TokenCategory.IntConst);
                        }

                        break;

                    case 3: // Barra (Divisão ou Comentário)
                        ConsumeChar();
                        char nextAfterSlash = (char)_reader.Peek();

                        if (nextAfterSlash == '/')
                        {
                            ConsumeChar();
                            _currentState = 14;
                        }
                        else if (nextAfterSlash == '*')
                        {
                            ConsumeChar();
                            _currentState = 15;
                        }
                        else
                        {
                            return CreateToken("/", "B21");
                        }

                        break;

                    case 4: 
                        character = ConsumeChar();
                        currentAtom.Append(character);

                        if (char.IsDigit((char)_reader.Peek()))
                        {
                            _currentState = 5;
                        }
                        else
                        {
                            return null;
                        }

                        break;

                    case 5: // Parte Fracionária do Float
                        character = ConsumeChar();
                        currentAtom.Append(character);

                        char nextFloat = (char)_reader.Peek();

                        if (nextFloat == 'e' || nextFloat == 'E')
                        {
                            _currentState = 16;
                        }
                        else if (!char.IsDigit(nextFloat))
                        {
                            return CreateToken(currentAtom.ToString(), Constants.TokenCategory.RealConst);
                        }

                        break;


                        #region [ Símbolos Compostos ]

                    
                    case 6: // ! ou !=
                        ConsumeChar(); // !
                        if ((char)_reader.Peek() == '=')
                        {
                            ConsumeChar(); // =
                            return CreateToken("!=", "B01");
                        }

                        return CreateToken("!", "B02");

                    case 7: // = ou ==
                        ConsumeChar(); // =
                        if ((char)_reader.Peek() == '=')
                        {
                            ConsumeChar(); // =
                            return CreateToken("==", "B13");
                        }

                        return CreateToken("=", "B12");

                    case 8: // < ou <=
                        ConsumeChar(); // <
                        if ((char)_reader.Peek() == '=')
                        {
                            ConsumeChar(); // =
                            return CreateToken("<=", "B15");
                        }

                        return CreateToken("<", "B14");

                    case 9: // > ou >=
                        ConsumeChar(); // >
                        if ((char)_reader.Peek() == '=')
                        {
                            ConsumeChar(); // =
                            return CreateToken(">=", "B17");
                        }

                        return CreateToken(">", "B16");

                    #endregion
                    
                    
                        #region [ Strings e Chars ]
                    
                    case 10: // Inicio String (")
                        ConsumeChar();
                        _currentState = 11;
                        break;

                    case 11: // Corpo String
                        character = ConsumeChar();
                        if (character == '"')
                        {
                            return CreateToken($"\"{currentAtom}\"", Constants.TokenCategory.StringConst);
                        }

                        if (character == '\n' || character == '\uffff') return null;

                        currentAtom.Append(character);
                        break;

                    case 12: // Inicio Char (')
                        ConsumeChar();
                        character = ConsumeChar();
                        currentAtom.Append(character);

                        if ((char)_reader.Peek() == '\'')
                        {
                            ConsumeChar();
                            return CreateToken($"'{currentAtom}'", Constants.TokenCategory.CharConst);
                        }

                        return null;
                    
                    #endregion

                    #region [ Comentários ]

                    case 14:
                        character = ConsumeChar();
                        if (character == '\n' || character == '\uffff')
                        {
                            _currentState = 0;
                            if (character == '\n') _currentLine++;
                        }

                        break;

                    case 15:
                        character = ConsumeChar();
                        if (character == '\n') _currentLine++;

                        if (character == '*')
                        {
                            if ((char)_reader.Peek() == '/')
                            {
                                ConsumeChar();
                                _currentState = 0;
                            }
                        }
                        else if (character == '\uffff')
                        {
                            return null;
                        }

                        break;

                    #endregion


                    #region [ Notação Científica ]

                    case 16:
                        character = ConsumeChar();
                        currentAtom.Append(character);

                        char nextExp = (char)_reader.Peek();
                        if (nextExp == '+' || nextExp == '-')
                        {
                            character = ConsumeChar();
                            currentAtom.Append(character);
                        }

                        if (char.IsDigit((char)_reader.Peek()))
                        {
                            _currentState = 17;
                        }
                        else
                        {
                            return null;
                        }

                        break;

                    case 17:
                        character = ConsumeChar();
                        currentAtom.Append(character);

                        if (!char.IsDigit((char)_reader.Peek()))
                        {
                            return CreateToken(currentAtom.ToString(), Constants.TokenCategory.RealConst);
                        }

                        break;

                    #endregion
                }
            }
        }

        #region [ Helpers ]

        private char ConsumeChar()
        {
            return (char)_reader.Read();
        }

        private Token CreateSingleCharToken(string code)
        {
            string lexeme = ((char)_reader.Read()).ToString();
            return CreateToken(lexeme, code);
        }

        private Token CreateToken(string lexeme, string code)
        {
            var originalSize = lexeme.Length;
            string finalLexeme = lexeme;

            if (lexeme.Length > 35)
            {
                finalLexeme = lexeme.Substring(0, 35);
            }

            var token = new Token(finalLexeme, code, _currentLine, originalSize);

            return token;
        }

        private void ProcessToken(Token token)
        {
            _tokens.Add(token);
        }

        private static bool IsWhiteSpace(char character)
        {
            return character == ' ' || character == '\n' || character == '\t' || character == '\r';
        }

        #endregion
    }
}
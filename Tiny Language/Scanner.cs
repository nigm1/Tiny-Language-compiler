using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    If, Int, Float, String, Read, Write, Repeat, Until, Elseif, Else, Then, Return, Endl, Comment,
    Semicolon, Comma, LParanthesis, RParanthesis, LCurlyBracket, RCurlyBracket,
    EqualOp, LessThanOp, GreaterThanOp, NotEqualOp, AssignmentOp, AndOp, OrOp,
    PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant, Main, End
}

namespace Tiny_Language
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }
    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("end", Token_Class.End);


            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurlyBracket);
            Operators.Add("}", Token_Class.RCurlyBracket);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
        }

        public void StartScanning(string SourceCode)
        {

            for (int i = 0; i < SourceCode.Length; i++)
            {

                string currentLexeme = "";
                char CurrentChar = SourceCode[i];

                //If whitespace
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                //If identifier or RW
                if (CurrentChar >= 'A' && CurrentChar <= 'z')
                {

                    while (char.IsLetterOrDigit(CurrentChar))

                    {
                        currentLexeme += CurrentChar;
                        CurrentChar = SourceCode[++i];
                    }
                    i--;
                }

                //If comments
                else if (CurrentChar == '/' && SourceCode[i + 1] == '*')
                {
                    i += 2;
                    CurrentChar = SourceCode[i];
                    currentLexeme += "/*";
                    while (i + 1 < SourceCode.Length && CurrentChar != '*' && SourceCode[i + 1] != '/')
                    {
                        currentLexeme += CurrentChar;
                        CurrentChar = SourceCode[++i];
                    }

                    if (CurrentChar == '*' && SourceCode[i + 1] == '/')
                        currentLexeme += "*/";
                    i++;
                }

                //If String
                else if (CurrentChar == '\"')
                {
                    currentLexeme += CurrentChar;
                    CurrentChar = SourceCode[++i];
                    while (CurrentChar != '\"')
                    {
                        currentLexeme += CurrentChar;
                        CurrentChar = SourceCode[++i];
                    }
                    currentLexeme += CurrentChar;
                }

                //If constant
                else if (char.IsDigit(CurrentChar))
                {
                    while (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '.')
                    {
                        currentLexeme += CurrentChar;
                        CurrentChar = SourceCode[++i];
                    }
                    i--;
                }

                // If operator 
                else
                {
                    currentLexeme += CurrentChar;

                    // If <> or := or && or || 
                    if (i != SourceCode.Length - 1)
                    {
                        char NextChar = SourceCode[i + 1];
                        if ((CurrentChar == '<' && NextChar == '>') || (CurrentChar == ':' && NextChar == '=')
                            || (CurrentChar == '&' && NextChar == '&') || (CurrentChar == '|' && NextChar == '|'))
                        {
                            currentLexeme += NextChar;
                            i++;
                        }
                    }

                }


                FindTokenClass(currentLexeme);
            }
            Tiny_Language.TokenStream = Tokens;

        }

        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;

            // is the lex a reserved word ? 
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            // is the lex an identifier ?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            // is the lex a constant ?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Constant;
                Tokens.Add(Tok);
            }
            // is the lex a string ?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            // is the lex an operator ?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }
            else
            {
                Errors.Error_List.Add("undefined :\t" + Lex);
            }

        }

        private bool isComment(string lex)
        {
            var re = new Regex(@"^\/\*.*\*\/$");
            return re.IsMatch(lex);
        }
        private bool isString(string lex)
        {
            var re = new Regex("^\".*\"$");
            return re.IsMatch(lex);
        }
        bool isIdentifier(string lex)
        {
            bool isValid = false;
            // Check if the lex is an identifier or not.
            var re = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                isValid = true;
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = false;
            // Check if the lex is a constant (Number) or not.
            var re = new Regex(@"^[0-9](\.[0-9]+)?$", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                isValid = true;
            return isValid;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace TextParser
{
    [PublicAPI]
    public class Parser
    {
        private readonly TextReader _reader;
        private readonly List<char> _peekedValue = new List<char>();
        private bool                _carriageReturn = false;
        public CharacterPosition CharacterPosition { get; private set; }

        public string Filename { get; }
        public bool Eof => Peek() == null;


        public Parser([NotNull] TextReader reader) : this(String.Empty, reader)
        {

        }

        public Parser(string filename, [NotNull] TextReader reader)
        {
            Filename = filename;
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            CharacterPosition = new CharacterPosition(1, 1);
        }


        public char? Peek(int ahead = 0)
        {
            if (_peekedValue.Count <= ahead)
            {
                var toRead = new char[ahead - _peekedValue.Count + 1];
                var count = _reader.Read(toRead, 0, toRead.Length);
                _peekedValue.AddRange(toRead.Take(count));
            }
            if (_peekedValue.Count <= ahead)
                return null;
            return _peekedValue[ahead];
        }

        public void Pop(int amount = 1)
        {
            if (amount == 0) return;
            Peek(amount);
            this.CharacterPosition = GetCharacterPositionAfter(amount);
            _peekedValue.RemoveRange(0, amount);
        }

        public CharacterPosition GetCharacterPositionAfter(int amount)
        {
            Peek(amount);
            CharacterPosition pos = CharacterPosition;
            for (int i = 0; i < amount; ++i)
            {
                if (_peekedValue[i] == '\n')
                {
                    if (_carriageReturn)
                    {
                        _carriageReturn = false;
                        continue;
                    }
                    pos.Line++;
                    pos.Character = 0;
                }
                if (_peekedValue[i] == '\r')
                {
                    _carriageReturn = true;
                    pos.Line++;
                    pos.Character = 0;
                }
                pos.Character++;
            }
            return pos;
        }

        public char Read()
        {
            var retval = Peek();
            if (retval == null) throw CompileException("Read past the end of the file");
            Pop();
            return retval.Value;
        }

        public string Read(int length)
        {
            Peek(length);
            if (_peekedValue.Count < length)
            {
                //point the exception at the end of the file
                Pop(_peekedValue.Count);
                throw CompileException("Read past the end of the file");
            }
            var retval = new StringBuilder(length);
            retval.Append(_peekedValue.Take(length).ToArray());
            Pop(length);
            return retval.ToString();
        }

        public CompilerException CompileException(string message)
        {
            return new CompilerException(message, Filename, CharacterPosition);
        }
        /// <summary>
        /// Checks to see if str is at the head. If so, it moves the head past it and returns true.
        /// Has side-effects.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MustUseReturnValue]
        public virtual bool Has([NotNull] string str)
        {
            if (!Check(str)) 
                return false;

            Pop(str.Length);
            return true;
        }
        /// <summary>
        /// Checks to see if str is at the head. If so, it returns true.
        /// Does NOT have side effects.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MustUseReturnValue]
        public virtual bool Check([NotNull] string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            return !str.Where((c, i) => Peek(i) != c).Any();
        }

        /// <summary>
        /// Checks to see if str is at the head. If so, it moves the head forward. If not, it throws a compile exception.
        /// Has side-effects.
        /// </summary>
        /// <param name="str"></param>
        public virtual void Expect([NotNull] string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (str.Where((c, i) => Peek(i) != c).Any())
            {
                throw CompileException($"Expected {str}");
            }
            Pop(str.Length);
        }
        /// <summary>
        /// Checks to see if any of the keys in the dictionary are at the head. If so, it moves the head forward and calls the associated value. If not, it throws a compile exception.
        /// Has side-effects.
        /// </summary>
        /// <param name="callbacks"></param>
        public void Expect(Dictionary<string, Action<Parser>> callbacks)
        {
            // NOTE: "Has" has side effects on success, so we need to stop after it passes. Thus we need to use "First*".
            var callback = (from p in callbacks where Has(p.Key) select p.Value).FirstOrDefault();
            if (callback == null) throw CompileException($"Expected one of the following: {string.Join(", ", callbacks.Keys)}.");
            callback(this);
        }

        static readonly Regex RxCStyleFirst = new Regex("[_a-zA-Z]");
        static readonly Regex RxCStyleRemaining = new Regex("[_a-zA-Z0-9]");

        [CanBeNull, MustUseReturnValue]
        public virtual Identifier PeekInteger()
        {
            int ahead = 0;
            char? c = Peek(ahead++);
            if (c == null
                || !(char.IsDigit(c.Value) || c == '-'))
                return null;
            var b = new StringBuilder();
            b.Append(c);

            while ((c = Peek(ahead++)) != null)
            {
                if (!char.IsDigit(c.Value))
                    break;
                b.Append(c);
            }
            return new Identifier(this, b.ToString());
        }

        public string ReadInteger()
        {
            var identifier = PeekInteger();
            if (identifier == null) throw CompileException("Expected identifier");
            identifier.Pop();
            return identifier.ToString();
        }

        [CanBeNull, MustUseReturnValue]
        public virtual Identifier PeekCStyleIdentifier()
        {
            int ahead = 0;
            char? c = Peek(ahead++);
            if (c == null
                || !RxCStyleFirst.IsMatch(new string(c.Value, count: 1)))
                return null;
            var b = new StringBuilder();
            b.Append(c);
            while ((c = Peek(ahead++)) != null)
            {
                if (!RxCStyleRemaining.IsMatch(new string(c.Value, 1)))
                    break;
                b.Append(c);
            }
            return new Identifier(this, b.ToString());
        }
        [MustUseReturnValue]
        public string ReadCStyleIdentifier()
        {
            var identifier = PeekCStyleIdentifier();
            if (identifier == null) throw CompileException("Expected identifier");
            identifier.Pop();
            return identifier.ToString();
        }

        [CanBeNull, MustUseReturnValue]
        public virtual Identifier PeekWord()
        {
            int ahead = 0;
            char? c;
            var b = new StringBuilder();
            while ((c = Peek(ahead++)) != null)
            {
                if (!Char.IsLetter(c.Value))
                {
                    if (Char.IsNumber(c.Value) || '_' == c.Value) return null;
                    break;
                }
                b.Append(c);
            }
            return b.Length != 0 ? new Identifier(this, b.ToString()) : null;
        }
        [MustUseReturnValue]
        public string ReadWord()
        {
            var identifier = PeekWord();
            if (identifier == null) throw CompileException("Expected word");
            identifier.Pop();
            return identifier.ToString();
        }
        
        [CanBeNull, MustUseReturnValue]
        public virtual Identifier PeekFilename(bool allowDirectory = false)
        {
            int ahead = 0;
            char? c;
            var b = new StringBuilder();
            bool isQuote = (Peek() ?? '\0') == '"';
            if (isQuote) ++ahead;

            while ((c = Peek(ahead++)) != null)
            {
                switch (c.Value)
                {
                    case ' ':
                        if(!isQuote) goto exitWhile;
                        break;
                    case '/':
                    case '\\':
                        if(!allowDirectory) goto exitWhile;
                        break;
                    case '\n':
                    case '\r':
                    case '\0':
                    case ':':
                    case '*':
                    case '?':
                    case '"':
                    case '<':
                    case '>':
                    case '|':
                        goto exitWhile;
                }
                b.Append(c);
            } exitWhile:
            if (isQuote && c != '"')
                throw new CompilerException(c == null ? "No closing quote in filename" : $"Invalid character '{c}' in filename", Filename, GetCharacterPositionAfter(ahead - 1));

            if (b.Length == 0) return null;

            return new Identifier(this, b.ToString(), isQuote ? 2 : 0);
        }
        [MustUseReturnValue]
        public string ReadFilename(bool allowDirectory = false)
        {
            var identifier = PeekFilename(allowDirectory);
            if (identifier == null) throw CompileException("Expected filename");
            identifier.Pop();
            return identifier.ToString();
        }

        public bool SkipWhitespace()
        {
            int ahead = 0;
            char? c;
            while ((c = Peek(ahead++)) != null)
            {
                if (!Char.IsWhiteSpace(c.Value)) break;
            }
            if (ahead == 1) return false;
            Pop(ahead - 1);
            return true;
        }

        public enum IsNewline
        {
            NoWhitespace,
            AtNewline,
            WhitespaceOnly
        }
        public IsNewline SkipWhitespaceToNewline()
        {
            int ahead = 0;
            char? c;
            while ((c = Peek(ahead++)) != null)
            {
                if (c == '\n' || c == '\r' || !Char.IsWhiteSpace(c.Value)) break;
            }
            if (ahead == 1) return IsNewline.NoWhitespace;
            Pop(ahead - 1);
            return c == '\n' || c == '\r' ? IsNewline.AtNewline : IsNewline.WhitespaceOnly;
        }

        public void ExpectScope([NotNull] string scope, Action readBody)
        {
            if (scope == null) throw new ArgumentNullException(nameof(scope));
            ExpectScope(scope, scope, readBody);
        }
        public void ExpectScope([NotNull] string scope, Func<bool> readBody)
        {
            if (scope == null) throw new ArgumentNullException(nameof(scope));
            ExpectScope(scope, scope, readBody);
        }

        public void ExpectScope([NotNull] string open, [NotNull] string close, Action readBody)
        {
            ExpectScope(open, close, ()=> { 
                readBody();
                return true;
            });
        }
        public void ExpectScope([NotNull] string open, [NotNull] string close, Func<bool> readBody)
        {
            if(!HasScope(open, close, readBody)) { 
                throw CompileException($"Expected {open}");
            }
        }
        

        public bool HasScope([NotNull] string scope, Action readBody)
        {
            if (scope == null) throw new ArgumentNullException(nameof(scope));
            return HasScope(scope, scope, readBody);
        }
        public bool HasScope([NotNull] string scope, Func<bool> readBody)
        {
            if (scope == null) throw new ArgumentNullException(nameof(scope));
            return  HasScope(scope, scope, readBody);
        }

        public bool HasScope([NotNull] string open, [NotNull] string close, Action readBody)
        {
            return HasScope(open, close, ()=> { 
                                      readBody();
                                      return true;
                                  });
        }
        /// <summary>
        /// Checks to see if there's a scope, and if so it will process it with readBody until it returns false or close is reached.
        /// Otherwise, it returns false.
        /// Has side-effects.
        /// </summary>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="readBody"></param>
        public bool HasScope([NotNull] string open, [NotNull] string close, Func<bool> readBody)
        {
            if (open  == null) throw new ArgumentNullException(nameof(open ));
            if (close == null) throw new ArgumentNullException(nameof(close));
            if (!Has(open)) return false;
            while (!Has(close))
            {
                if (readBody() && !Eof) continue;

                Expect(close);
                break;
            }

            return true;
        }

        [NotNull]
        public string ReadLine()
        {
            int ahead = 0;
            var b = new StringBuilder();
            char? c;
            while ((c = Peek(ahead++)) != null)
            {
                if (c.Value == '\r')
                {
                    c = Peek(ahead++);
                    if (c == '\n') ++ahead;
                    break;
                }
                if (c.Value == '\n')
                {
                    ++ahead;
                    break;
                }
                b.Append(c);
            }
            Pop(ahead - 1);
            return b.ToString();
        }

        [MustUseReturnValue]
        public virtual bool HasNewline()
        {
            switch (Peek())
            {
                case '\r':
                    Pop(Peek(1) == '\n' ? 2 : 1);
                    return true;
                case '\n':
                    Pop();
                    return true;
                default:
                    return false;
            }
        }

        public void ExpectNewline()
        {
            if (!HasNewline()) throw CompileException("Expected a newline");
        }

        public void SkipNewline()
        {
            // ReSharper disable once MustUseReturnValue
            HasNewline();
        }
        
        public bool HasWord(string expected)
        {
            var actual = PeekWord();
            if (actual            == null    ) return false;
            if (actual.ToString() != expected) return false;
            actual.Pop();
            return true;
        }
        public void ExpectWord(string expected)
        {
            var actual = PeekWord();
            if (actual            == null    ) throw CompileException($"Expected '{expected}'"                );
            if (actual.ToString() != expected) throw CompileException($"Expected '{expected}', got '{actual}'");
            actual.Pop();
        }

        public string ReadQuotedString()
        {
            string result = null;
            ExpectScope("\"", () =>
            {
                char? c;
                int ahead = 0;
                while ((c = Peek(ahead++)) != null)
                {
                    if (c == '"') break;
                    if (c == '\\')
                    {
                        ++ahead;
                    }
                }
                result = Read(ahead - 1);
            });
            return result;
        }

        public bool SkipWhitespaceAndCStyleComments()
        {
            bool retval = false;
            while (true)
            {
                retval = SkipWhitespace() || retval;
                if (Has("/*"))
                {
                    while (!Eof && !Has("*/"))
                    {
                        Pop();
                    }
                    retval = true;
                    continue;
                }
                if (Has("//"))
                {
                    ReadLine();
                    SkipWhitespace();
                    retval = true;
                    continue;
                }
                break;
            }
            return retval;
        }
    }
}

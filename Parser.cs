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
    public partial class Parser
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

        public Parser(string filename, [NotNull] TextReader reader, CharacterPosition? characterPosition = null)
        {
            Filename = filename;
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            
            CharacterPosition = characterPosition ?? new CharacterPosition(1, 1);
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

        public virtual bool SkipIgnoredText() { return SkipWhitespace(); }

        /// <summary>
        /// Checks to see if str is at the head. If so, it moves the head past it and returns true.
        /// Has side-effects.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="skip">Parameter used by derived classes to suspend any skipping</param>
        /// <returns></returns>
        [MustUseReturnValue]
        public bool Has([NotNull] string str, bool skip=true)
        {
            if (!Check(str, skip)) 
                return false;

            Pop(str.Length);
            return true;
        }
        [MustUseReturnValue]
        public bool Has(char c, bool skip =true)
        {
            if (!Check(c, skip)) 
                return false;

            Pop();
            return true;
        }

        /// <summary>
        /// Checks to see if str is at the head. If so, it returns true.
        /// Does NOT have side effects.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="skip">Parameter used by derived classes to suspend any skipping</param>
        /// <returns></returns>
        [MustUseReturnValue]
        public bool Check([NotNull] string str, bool skip=true)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (skip) this.SkipIgnoredText();
            return !str.Where((c, i) => Peek(i) != c).Any();
        }
        [MustUseReturnValue]
        public bool Check(char c, bool skip =true)
        {
            if (skip) this.SkipIgnoredText();
            return Peek() == c;
        }

        /// <summary>
        /// Checks to see if str is at the head. If so, it moves the head forward. If not, it throws a compile exception.
        /// Has side-effects.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="skip">Parameter used by derived classes to suspend any skipping</param>
        public void Expect([NotNull] string str, bool skip=true)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (!Check(str, skip))
            {
                throw CompileException($"Expected {str}");
            }
            Pop(str.Length);
        }

        public void Expect(char str, bool skip = true)
        {
            if(!Check(str, skip))
                throw CompileException($"Expected {str}");
            Pop();
        }

        /// <summary>
        /// Checks to see if any of the keys in the dictionary are at the head. If so, it moves the head forward and calls the associated value. If not, it throws a compile exception.
        /// Has side-effects.
        /// </summary>
        /// <param name="callbacks">The expected keywords and their associated callbacks</param>
        /// <param name="skip">If true, will ensure ignored text is skipped before parsing the tokens</param>
        public void Expect(Dictionary<string, Action<Parser>> callbacks, bool skip=true)
        {
            if(skip) SkipIgnoredText();
            // NOTE: "Has" has side effects on success, so we need to stop after it passes. Thus we need to use "First*".
            var callback = (from p in callbacks where Has(p.Key, skip: false) select p.Value).FirstOrDefault();
            if (callback == null) throw CompileException($"Expected one of the following: {string.Join(", ", callbacks.Keys)}.");
            callback(this);
        }

        /// <param name="skip">Parameter used by derived classes to suspend any skipping</param>
        [CanBeNull, MustUseReturnValue]
        public Identifier PeekInteger(bool skip=true)
        {
            if(skip) SkipIgnoredText();
            int ahead = 0;
            char? c = Peek(ahead++);
            if (c == null || !(char.IsDigit(c.Value) || c == '-'))
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

        public string ReadInteger(bool skip =true)
        {
            var identifier = PeekInteger(skip);
            if (identifier == null) throw CompileException("Expected identifier");
            identifier.Pop();
            return identifier.ToString();
        }
        
        static readonly Regex RxCStyleFirst     = new Regex("[_a-zA-Z]");
        static readonly Regex RxCStyleRemaining = new Regex("[_a-zA-Z0-9]");
        [CanBeNull, MustUseReturnValue]
        public Identifier PeekCStyleIdentifier(bool skip=true)
        {
            if(skip) SkipIgnoredText();
            int ahead = 0;
            char? c = Peek(ahead++);
            if (c == null || !RxCStyleFirst.IsMatch(new string(c.Value, count: 1)))
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
        public string ReadCStyleIdentifier(bool skip =true)
        {
            var identifier = PeekCStyleIdentifier(skip);
            if (identifier == null) throw CompileException("Expected identifier");
            identifier.Pop();
            return identifier.ToString();
        }

        [CanBeNull, MustUseReturnValue]
        public Identifier PeekWord(bool skip =true)
        {
            if(skip) SkipIgnoredText();
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
        public string ReadWord(bool skip =true)
        {
            var identifier = PeekWord(skip);
            if (identifier == null) throw CompileException("Expected word");
            identifier.Pop();
            return identifier.ToString();
        }
        
        [CanBeNull, MustUseReturnValue]
        public Identifier PeekFilename(bool allowDirectory = false, bool skip =true)
        {
            if(skip) SkipIgnoredText();
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
        public string ReadFilename(bool allowDirectory = false, bool skip =true)
        {
            var identifier = PeekFilename(allowDirectory,skip);
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
        public bool HasNewline()
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

        public string ReadQuotedString(char quote='"')
        {
            string result = null;
            var expectedQuote = new string(quote, 1);
            ExpectScope(expectedQuote, p =>
            {
                char? c;
                int ahead = 0;
                while ((c = p.Peek(ahead++)) != null)
                {
                    if (c == quote) break;
                    if (c == '\\')
                    {
                        ++ahead;
                    }
                }
                result = p.Read(ahead - 1);
            }, skipInside:false);
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

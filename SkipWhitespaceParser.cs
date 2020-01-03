using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace TextParser
{
    public class SkipWhitespaceParser : Parser
    {
        /// <inheritdoc />
        public SkipWhitespaceParser([NotNull] TextReader reader) : base(reader) { }

        /// <inheritdoc />
        public SkipWhitespaceParser(string filename, [NotNull] TextReader reader) : base(filename, reader) { }

        public override bool Has(string str) { 
            SkipWhitespace();
            return base.Has(str);
        }

        public override bool Check(string str)
        {
            SkipWhitespace();
            return base.Check(str);
        }

        public override void Expect(string str)
        {
            SkipWhitespace();
            base.Expect(str);
        }

        /// <inheritdoc />
        public override bool HasNewline()
        {
            
            int ahead = 0;
            char? c;
            while ((c = Peek(ahead++)) != null)
            {
                switch (c.Value)
                {
                case '\r':
                    Pop(Peek(1) == '\n' ? 2 : 1);
                    return true;
                case '\n':
                    Pop();
                    return true;
                default:
                    if (!Char.IsWhiteSpace(c.Value)) 
                        return false;
                    break;
                }
            }
            //Undo the last increment
            --ahead;
            if (ahead == 0) return false;
            Pop(ahead);
            return true;
        }

        /// <inheritdoc />
        public override Identifier PeekCStyleIdentifier()
        {
            SkipWhitespace();
            return base.PeekCStyleIdentifier();
        }

        /// <inheritdoc />
        public override Identifier PeekFilename(bool allowDirectory = false)
        {
            SkipWhitespace(); 
            return base.PeekFilename(allowDirectory);
        }

        /// <inheritdoc />
        public override Identifier PeekInteger()
        {
            SkipWhitespace(); 
            return base.PeekInteger();
        }

        /// <inheritdoc />
        public override Identifier PeekWord()
        {
            SkipWhitespace();
            return base.PeekWord();
        }
    }
}

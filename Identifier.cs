using System;

namespace TextParser
{
    public class Identifier : IDisposable
    {
        private readonly string            _text;
        private readonly Parser            _parser;
        private readonly CharacterPosition _startPosition;
        private readonly int               _amountToPop;

        public Identifier(Parser parser, string text, int additonalLength = 0)
        {
            _parser        =  parser;
            _text          =  text;
            _startPosition = _parser.CharacterPosition;
            _amountToPop   = text.Length + additonalLength;
        }

        public override string ToString()
        {
            return _text;
        }

        public void Pop()
        {
            if(_parser.CharacterPosition != _startPosition)
                throw new InvalidOperationException("Position mismatch for identifier");
            _parser.Pop(_amountToPop);
        }

        void IDisposable.Dispose()
        {
            Pop();
        }
    }
}
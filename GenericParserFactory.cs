using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace TextParser
{
    [PublicAPI]
    public class GenericParserFactory<TParser> : IParserFactory<TParser> where TParser : Parser
    {
        public string Filename { get; }
        public TextReader Reader { get; }

        public GenericParserFactory(string filename, TextReader reader)
        {
            Filename = filename;
            Reader = reader;
        }
        public TParser CreateDefaultParser()
        {
            return (TParser)Activator.CreateInstance(typeof(TParser), Filename, Reader, null);
        }

        public TParser CreateSubParser(string substring, CharacterPosition startingPosition)
        {
            var reader = new StringReader(substring);
            return (TParser)Activator.CreateInstance(typeof(TParser), Filename, reader, startingPosition);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParser
{
    public interface IParserFactory<out TParser> where TParser:Parser
    {
        TParser CreateDefaultParser();
        TParser CreateSubParser(string substring, CharacterPosition startingPosition);
    }
}

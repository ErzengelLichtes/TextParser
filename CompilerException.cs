using System;
using System.Net;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace TextParser
{
    [PublicAPI]
    public class CompilerException : Exception
    {
        public string ErrorMessage { get; }
        public string Filename { get; }
        public CharacterPosition CharacterPosition { get; }

        public CompilerException(string errorMessage, string filename, CharacterPosition characterPosition) : base($"{filename}:{characterPosition.Line}:{characterPosition.Character} : {errorMessage}")
        {
            ErrorMessage = errorMessage;
            Filename = filename;
            CharacterPosition = characterPosition;
        }

        public CompilerException(string errorMessage, string filename, CharacterPosition characterPosition, Exception innerException) : base($"{filename}:{characterPosition.Line}:{characterPosition.Character} : {errorMessage}", innerException)
        {
            ErrorMessage = errorMessage;
            Filename = filename;
            CharacterPosition = characterPosition;
        }

        public CompilerException()
        {
        }

        public CompilerException(string message) : base(message)
        {
        }

        public CompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CompilerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ErrorMessage = info.GetString(nameof(ErrorMessage));
            Filename = info.GetString(nameof(Filename));
            CharacterPosition = (CharacterPosition)info.GetValue(nameof(CharacterPosition), typeof(CharacterPosition));
        }
    }
}
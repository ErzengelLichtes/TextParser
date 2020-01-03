using System;
using JetBrains.Annotations;

namespace TextParser
{
    [PublicAPI, Serializable]
    public struct CharacterPosition
    {
        public CharacterPosition(int line, int character)
        {
            Character = character;
            Line = line;
        }

        public int Character { get; set; }
        public int Line      { get; set; }
        public override string ToString()
        {
            return $"Line: {Line} Character: {Character}";
        }

        public bool Equals(CharacterPosition other) => Character == other.Character && Line == other.Line;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CharacterPosition position && Equals(position);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (Character * 397) 
                    ^  Line;
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        public static bool operator ==(CharacterPosition left, CharacterPosition right) => left.Equals(right);

        public static bool operator !=(CharacterPosition left, CharacterPosition right) => !left.Equals(right);
    }
}
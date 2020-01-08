using System;
using JetBrains.Annotations;

namespace TextParser {
    partial class Parser {
        public void ExpectScope(char scope, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(scope, skipBefore)) throw CompileException($"Expected {scope}");
            while(!Has(scope, skipInside) && !Eof)
            {
                readBody(this);
                Expect(scope);
                break;
            }
            
        }
        public void ExpectScope(char scope, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(scope, skipBefore)) throw CompileException($"Expected {scope}");
            while(!Has(scope, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(scope);
                break;
            }
            
        }
        public void ExpectScope([NotNull] string scope, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(scope == null) throw new ArgumentNullException(nameof(scope));
            if(!Has(scope, skipBefore)) throw CompileException($"Expected {scope}");
            while(!Has(scope, skipInside) && !Eof)
            {
                readBody(this);
                Expect(scope);
                break;
            }
            
        }
        public void ExpectScope([NotNull] string scope, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(scope == null) throw new ArgumentNullException(nameof(scope));
            if(!Has(scope, skipBefore)) throw CompileException($"Expected {scope}");
            while(!Has(scope, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(scope);
                break;
            }
            
        }
        public void ExpectScope(char open, char close, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(open, skipBefore)) throw CompileException($"Expected {open}");
            while(!Has(close, skipInside) && !Eof)
            {
                readBody(this);
                Expect(close);
                break;
            }
            
        }
        public void ExpectScope(char open, char close, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(open, skipBefore)) throw CompileException($"Expected {open}");
            while(!Has(close, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(close);
                break;
            }
            
        }
        public void ExpectScope([NotNull] string open, [NotNull] string close, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(open == null) throw new ArgumentNullException(nameof(open));
            if(close == null) throw new ArgumentNullException(nameof(close));
            if(!Has(open, skipBefore)) throw CompileException($"Expected {open}");
            while(!Has(close, skipInside) && !Eof)
            {
                readBody(this);
                Expect(close);
                break;
            }
            
        }
        public void ExpectScope([NotNull] string open, [NotNull] string close, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(open == null) throw new ArgumentNullException(nameof(open));
            if(close == null) throw new ArgumentNullException(nameof(close));
            if(!Has(open, skipBefore)) throw CompileException($"Expected {open}");
            while(!Has(close, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(close);
                break;
            }
            
        }
        public bool HasScope(char scope, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(scope, skipBefore)) return false;
            while(!Has(scope, skipInside) && !Eof)
            {
                readBody(this);
                Expect(scope);
                break;
            }
            return true;
        }
        public bool HasScope(char scope, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(scope, skipBefore)) return false;
            while(!Has(scope, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(scope);
                break;
            }
            return true;
        }
        public bool HasScope([NotNull] string scope, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(scope == null) throw new ArgumentNullException(nameof(scope));
            if(!Has(scope, skipBefore)) return false;
            while(!Has(scope, skipInside) && !Eof)
            {
                readBody(this);
                Expect(scope);
                break;
            }
            return true;
        }
        public bool HasScope([NotNull] string scope, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(scope == null) throw new ArgumentNullException(nameof(scope));
            if(!Has(scope, skipBefore)) return false;
            while(!Has(scope, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(scope);
                break;
            }
            return true;
        }
        public bool HasScope(char open, char close, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(open, skipBefore)) return false;
            while(!Has(close, skipInside) && !Eof)
            {
                readBody(this);
                Expect(close);
                break;
            }
            return true;
        }
        public bool HasScope(char open, char close, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(!Has(open, skipBefore)) return false;
            while(!Has(close, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(close);
                break;
            }
            return true;
        }
        public bool HasScope([NotNull] string open, [NotNull] string close, Action<Parser> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(open == null) throw new ArgumentNullException(nameof(open));
            if(close == null) throw new ArgumentNullException(nameof(close));
            if(!Has(open, skipBefore)) return false;
            while(!Has(close, skipInside) && !Eof)
            {
                readBody(this);
                Expect(close);
                break;
            }
            return true;
        }
        public bool HasScope([NotNull] string open, [NotNull] string close, Func<Parser, bool> readBody, bool skipBefore = true, bool skipInside = true)
        {
            if(open == null) throw new ArgumentNullException(nameof(open));
            if(close == null) throw new ArgumentNullException(nameof(close));
            if(!Has(open, skipBefore)) return false;
            while(!Has(close, skipInside) && !Eof)
            {
                if(readBody(this)) continue;
                Expect(close);
                break;
            }
            return true;
        }
    }
}


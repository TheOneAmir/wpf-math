using System;
using System.Collections;
using System.Collections.Generic;

namespace WpfMath
{
    // Atom representing symbol (non-alphanumeric character).
    internal readonly struct SymbolAtom : ICharSymbol
    {
        /// <summary>
        /// Special name of empty delimiter symbol that shouldn't be rendered.
        /// </summary>
        internal const string EmptyDelimiterName = "_emptyDelimiter";

        // Dictionary of definitions of all symbols, keyed by name.
        private static IDictionary<string, SymbolAtom> symbols;

        // Set of all valid symbol types.
        private static BitArray validSymbolTypes;

        static SymbolAtom()
        {
            var symbolParser = new TexSymbolParser();
            symbols = symbolParser.GetSymbols();

            validSymbolTypes = new BitArray(16);
            validSymbolTypes.Set((int)TexAtomType.Ordinary, true);
            validSymbolTypes.Set((int)TexAtomType.BigOperator, true);
            validSymbolTypes.Set((int)TexAtomType.BinaryOperator, true);
            validSymbolTypes.Set((int)TexAtomType.Relation, true);
            validSymbolTypes.Set((int)TexAtomType.Opening, true);
            validSymbolTypes.Set((int)TexAtomType.Closing, true);
            validSymbolTypes.Set((int)TexAtomType.Punctuation, true);
            validSymbolTypes.Set((int)TexAtomType.Accent, true);
        }

        public static SymbolAtom GetAtom(string name, SourceSpan source)
        {
            try
            {
                var symbol = symbols[name];
                return new SymbolAtom(symbol, symbol.Type, source);
            }
            catch (KeyNotFoundException)
            {
                throw new SymbolNotFoundException(name);
            }
        }

        public static bool TryGetAtom(SourceSpan name, out SymbolAtom? atom)
        {
            if (symbols.TryGetValue(name.ToString(), out var temp))
            {
                atom = new SymbolAtom(temp, temp.Type, name);
                return true;
            }

            atom = null;
            return false;
        }

        public TexAtomType Type { get; }
        public TexAtomType GetLeftType() => Type;
        public TexAtomType GetRightType() => Type;

        public SourceSpan Source { get; }
        public bool IsTextSymbol => false;

        public SymbolAtom(SymbolAtom symbolAtom, TexAtomType type, SourceSpan source)
        {
            if (!validSymbolTypes[(int)type])
                throw new ArgumentException("The specified type is not a valid symbol type.", "type");
            this.Type = type;
            this.Name = symbolAtom.Name;
            this.IsDelimeter = symbolAtom.IsDelimeter;
            this.Source = source;
        }

        public SymbolAtom(string name, TexAtomType type, bool isDelimeter)
        {
            this.Type = type;
            this.Name = name;
            this.IsDelimeter = isDelimeter;
            this.Source = null;
        }

        public bool IsDelimeter { get; }

        public string Name { get; }

        public Box CreateBox(TexEnvironment environment)
        {
            return new CharBox(environment, environment.MathFont.GetCharInfo(this.Name, environment.Style));
        }

        public ITeXFont GetStyledFont(TexEnvironment environment) => environment.MathFont;

        public CharFont GetCharFont(ITeXFont texFont)
        {
            // Style is irrelevant here.
            return texFont.GetCharInfo(Name, TexStyle.Display).GetCharacterFont();
        }
    }
}

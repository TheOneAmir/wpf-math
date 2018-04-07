namespace WpfMath
{
    // Atom representing character that does not depend on text style.
    internal class FixedCharAtom : CharSymbol
    {
        public FixedCharAtom(SourceSpan source, CharFont charFont) : base(source)
        {
            this.CharFont = charFont;
        }

        public CharFont CharFont { get; }

        public override CharFont GetCharFont(ITeXFont texFont)
        {
            return this.CharFont;
        }

        public override Box CreateBox(TexEnvironment environment, bool isTextSymbol)
        {
            var charInfo = environment.MathFont.GetCharInfo(this.CharFont, environment.Style);
            return new CharBox(environment, charInfo);
        }
    }
}

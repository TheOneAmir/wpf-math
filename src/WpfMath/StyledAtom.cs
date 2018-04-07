using System.Windows.Media;

namespace WpfMath
{
    // Atom specifying graphical style.
    internal class StyledAtom : Atom, IRow
    {
        public StyledAtom(SourceSpan source, Atom atom, Brush backgroundColor, Brush foregroundColor) : base(source)
        {
            this.RowAtom = new RowAtom(atom);
            this.Background = backgroundColor;
            this.Foreground = foregroundColor;
        }

        public DummyAtom PreviousAtom => this.RowAtom.PreviousAtom;

        // RowAtom to which colors are applied.
        public RowAtom RowAtom { get; }

        public Brush Background { get; }

        public Brush Foreground { get; }

        public override Box CreateBox(TexEnvironment environment)
        {
            var newEnvironment = environment.Clone();
            if (this.Background != null)
                newEnvironment.Background = this.Background;
            if (this.Foreground != null)
                newEnvironment.Foreground = this.Foreground;
            return this.RowAtom.CreateBox(newEnvironment);
        }

        public override TexAtomType GetLeftType()
        {
            return this.RowAtom.GetLeftType();
        }

        public override TexAtomType GetRightType()
        {
            return this.RowAtom.GetRightType();
        }

        public StyledAtom Clone(
            SourceSpan source = null,
            RowAtom rowAtom = null,
            Brush background = null,
            Brush foreground = null)
        {
            return new StyledAtom(
                source ?? this.Source,
                rowAtom ?? this.RowAtom,
                background ?? this.Background,
                foreground ?? this.Foreground);
        }
    }
}

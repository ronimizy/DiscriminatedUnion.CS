using System;
using System.Text;

namespace DiscriminatedUnion.Generators.Utility
{
    public readonly struct SyntaxBuilder : IDisposable
    {
        private readonly StringBuilder _builder;
        private readonly int _indentation;
        private readonly bool _needToDispose;

        public SyntaxBuilder(StringBuilder builder)
        {
            _builder = builder;
            _indentation = 0;
            _needToDispose = false;
        }

        private SyntaxBuilder(StringBuilder builder, int indentation)
        {
            _builder = builder;
            _indentation = indentation;
            _needToDispose = true;

            builder.Append('\t', indentation - 1);
            builder.AppendLine("{");
        }

        public void Dispose()
        {
            if (!_needToDispose)
                return;

            _builder.Append('\t', _indentation - 1);
            _builder.AppendLine("}");
        }

        public void AppendLine(string value)
        {
            _builder.Append('\t', _indentation);
            _builder.AppendLine(value);
        }

        public void AppendLine()
        {
            _builder.AppendLine();
        }

        public void Append(string value)
        {
            _builder.Append(value);
        }

        public SyntaxBuilder OpenBraces() => new SyntaxBuilder(_builder, _indentation + 1);
    }
}
namespace Tharga.Test.Toolkit
{
    internal sealed class AncestorPath
    {
        public static readonly AncestorPath Empty = new AncestorPath(null, null);

        private readonly object _value;
        private readonly AncestorPath _parent;

        private AncestorPath(object value, AncestorPath parent)
        {
            _value = value;
            _parent = parent;
        }

        public AncestorPath With(object value)
        {
            return new AncestorPath(value, this);
        }

        public bool HasEqual(object value)
        {
            var type = value.GetType();
            for (var node = this; node._parent != null; node = node._parent)
            {
                if (node._value.GetType() == type && node._value.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

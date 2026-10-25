using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tharga.Test.Toolkit
{
    internal sealed class ReferenceIdentityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceIdentityComparer Instance = new ReferenceIdentityComparer();

        private ReferenceIdentityComparer()
        {
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}

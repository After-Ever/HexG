using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class EmptyRegion : IReadOnlyRegion
    {
        public int Count => 0;

        public bool Contains(HexPoint point) => false;

        public IEnumerator<HexPoint> GetEnumerator()
            => new EmptyEnum();

        public int MaxInDirection(Direction direction) => 0;

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        class EmptyEnum : IEnumerator<HexPoint>
        {
            public HexPoint Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose() { }

            public bool MoveNext()
                => false;

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}

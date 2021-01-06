using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public interface IEntityHexMapEntity<T> where T : class
    {
        bool CanBeReplaced { get; }

        void Added(EntityHexMapHandle<T> handle);
        void Moved(HexPoint lastPosition);
        void Removed(HexPoint lastPosition);
    }
}

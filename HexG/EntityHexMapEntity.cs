using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public interface IEntityHexMapEntity
    {
        bool CanBeReplaced { get; }

        void Added(EntityHexMapHandle handle);
        void Moved(HexPoint lastPosition);
        void Removed(HexPoint lastPosition);
    }
}

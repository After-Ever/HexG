using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public abstract class EntityHexMapEntity
    {
        public EntityHexMapHandle Handle { get; private set; }

        public virtual bool CanBeReplaced { get => true; }

        protected virtual void Added() { }
        protected virtual void Moved(HexPoint lastPosition) { }
        protected virtual void Removed(HexPoint lastPosition) { }

        internal void _AddedToEntityMap(EntityHexMapHandle handle)
        {
            Handle = handle;
            Added();
        }

        internal void _Moved(HexPoint lastPosition)
            => Moved(lastPosition);

        internal void _Removed(HexPoint lastPosition)
        {
            Handle = null;
            Removed(lastPosition);
        }
    }
}

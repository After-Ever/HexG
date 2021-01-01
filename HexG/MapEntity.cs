using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class MapEntity
    {
        public EntityHexMap Map { get; private set; }

        HexPoint? position;
        /// <summary>
        /// The position of this within <see cref="Map"/>.
        /// 
        /// If this entity is not currently contained in a <see cref="EntityHexMap"/>, this will throw
        /// an Exception.
        /// </summary>
        public HexPoint Position => position ?? throw new Exception("MapEntity is not in a Map.");

        /// <summary>
        /// Durring Move and Add operations, can this be replaced?
        /// If false, those operations will fail, resulting in no change to the map.
        /// </summary>
        public virtual bool CanBeReplaced => true;

        internal void SetPosition(HexPoint position)
        {
            this.position = position;
        }

        internal void SetMap(EntityHexMap map)
        {
            Map = map;
            if (map == null)
                position = null;
        }

        // Event methods:

        /// <summary>
        /// Called after this entity has been added to an <see cref="EntityHexMap"/>.
        /// </summary>
        public virtual void WasAdded() { }
        /// <summary>
        /// Called after this entity has been moved within the same <see cref="EntityHexMap"/>.
        /// </summary>
        /// <param name="lastPosition">The previous position before being moved.</param>
        public virtual void WasMoved(HexPoint lastPosition) { }
        /// <summary>
        /// Called after this entity has been removed.
        /// </summary>
        /// <param name="lastPosition">The position this entity had before being removed.</param>
        public virtual void WasRemoved(HexPoint lastPosition) { }


        // Map manipulation methods:

        /// <summary>
        /// Add this entity to the specified <see cref="EntityHexMap"/> at the
        /// specified position.
        /// 
        /// This operation will cause event methods to be called.
        /// </summary>
        /// <param name="newMap"></param>
        /// <param name="position"></param>
        public bool AddTo(EntityHexMap newMap, HexPoint position)
        {
            if (Map != null)
                throw new Exception("Must not be in a map to call this operation.");

            return newMap.Add(this, position);
        }

        /// <summary>
        /// Move this to <paramref name="destination"/> within the same <see cref="EntityHexMap"/>.
        /// 
        /// Must be within a map, or else this will throw.
        /// 
        /// This operation will cause event methods to be called.
        /// </summary>
        /// <param name="destination"></param>
        public bool MoveTo(HexPoint destination)
        {
            ThrowIfNoMap();

            return Map.Move(Position, destination);
        }

        /// <summary>
        /// Remove this from its <see cref="EntityHexMap"/>.
        /// 
        /// Must be within a map, or else this will throw.
        /// 
        /// This operation will cause event methods to be called.
        /// </summary>
        public bool Remove()
        {
            ThrowIfNoMap();

            return Map.Remove(Position);
        }

        void ThrowIfNoMap()
        {
            if (Map == null)
                throw new NotInMapException();
        }
    }

    public class NotInMapException : Exception
    {
        public NotInMapException()
            : base("Cannot perform opperation if not part of a map.") { }
    }
}

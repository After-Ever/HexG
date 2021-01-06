using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public abstract class EntityHexMapHandle
    {
        public abstract HexPoint Position { get; protected set; }

        /// <summary>
        /// Move this to <paramref name="destination"/> within the same <see cref="EntityHexMap"/>.
        /// 
        /// Must be within a map, or else this will throw.
        /// 
        /// This operation will cause event methods to be called.
        /// </summary>
        /// <param name="destination"></param>
        public abstract bool MoveTo(HexPoint destination);

        /// <summary>
        /// Remove this from its <see cref="EntityHexMap"/>.
        /// 
        /// Must be within a map, or else this will throw.
        /// 
        /// This operation will cause event methods to be called.
        /// </summary>
        public abstract bool Remove();
    }

    public class EntityHexMapHandle<T> : EntityHexMapHandle where T : class
    {
        public readonly EntityHexMap<T> Map;
        public sealed override HexPoint Position { get; protected set; }
        public readonly T Data;

        public EntityHexMapHandle(EntityHexMap<T> map, T data, HexPoint position)
        {
            Map = map;
            Position = position;
            Data = data;
        }

        /// <summary>
        /// Move this to <paramref name="destination"/> within the same <see cref="EntityHexMap"/>.
        /// 
        /// Must be within a map, or else this will throw.
        /// 
        /// This operation will cause event methods to be called.
        /// </summary>
        /// <param name="destination"></param>
        public override bool MoveTo(HexPoint destination)
        {
            return Map.Move(Position, destination);
        }

        /// <summary>
        /// Remove this from its <see cref="EntityHexMap"/>.
        /// 
        /// Must be within a map, or else this will throw.
        /// 
        /// This operation will cause event methods to be called.
        /// </summary>
        public override bool Remove()
        {
            return Map.Remove(Position);
        }
    }
}

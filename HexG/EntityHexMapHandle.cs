using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class EntityHexMapHandle<T> where T : class
    {
        public readonly EntityHexMap<T> Map;
        public HexPoint Position { get; internal set; }
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
        public bool MoveTo(HexPoint destination)
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
        public void Remove()
        {
            if (!Map.Remove(Position))
                throw new Exception("EntityHexMapHandle should never fail to remove!");
        }
    }
}

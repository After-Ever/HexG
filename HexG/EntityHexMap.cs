using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class EntityHexMap
    {
        IHexMap<MapEntity> baseMap;
        IRegion allowedRegion;
        IRegion disallowedRegion;

        /// <summary>
        /// Create a new, empty <see cref="EntityHexMap"/>.
        /// </summary>
        /// <param name="baseMap">The base map that will be used to store the <see cref="MapEntity"/>.
        /// Must be empty.</param>
        /// <param name="allowedRegion">If provided, entities can only inhabit positions within this region.</param>
        /// <param name="disallowedRegion">If provided, entities cannot inhabit positions within this region.</param>
        public EntityHexMap(IHexMap<MapEntity> baseMap, IRegion allowedRegion = null, IRegion disallowedRegion = null)
        {
            this.baseMap = baseMap ?? throw new ArgumentNullException();
            this.allowedRegion = allowedRegion;
            this.disallowedRegion = disallowedRegion;
            
            if (!baseMap.IsEmpty)
                throw new Exception("Base map must be empty!");
        }

        /// <summary>
        /// Add <paramref name="entity"/> to the map at <paramref name="position"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <param name="replaced"></param>
        /// <returns>Whether the addition was succesful. When false is returned,
        /// <paramref name="entity"/> has not been added, and the map has not changed.</returns>
        public bool Add(MapEntity entity, HexPoint position, out MapEntity replaced)
        {
            if (entity == null)
                throw new ArgumentNullException();
            if (entity.Map != null)
                throw new Exception("Cannot add an entity already apart of a map.");
            
            if (!(allowedRegion?.Contains(position) ?? true) 
                || (disallowedRegion?.Contains(position) ?? false))
            {
                replaced = null;
                return false;
            }

            replaced = baseMap[position];

            // Fail the addition if the target position has an entity which cannot be replaced.
            if (!(replaced?.CanBeReplaced ?? true))
            {
                replaced = null;
                return false;
            }

            baseMap[position] = entity;

            // Update the replaced.
            replaced?.SetMap(null);
            replaced?.WasRemoved(position);

            // Update the added.
            entity.SetMap(this);
            entity.SetPosition(position);
            entity.WasAdded();

            return true;
        }

        public bool Add(MapEntity entity, HexPoint position)
        {
            MapEntity _;
            return Add(entity, position, out _);
        }

        /// <summary>
        /// Move the entity at <paramref name="source"/> to <paramref name="target"/>.
        /// 
        /// If there is no entity at <paramref name="source"/>, the operation fails.
        /// If there is an entity at <paramref name="target"/>, and it cannot be replaced, 
        /// the operation fails.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="moved"></param>
        /// <param name="replaced"></param>
        /// <returns>Whether the move was succesful. When false is returned, the move did not occur, and
        /// the map is unchanged.</returns>
        public bool Move(HexPoint source, HexPoint target, out MapEntity moved, out MapEntity replaced)
        {
            // Only check target, as if source is in an invalid region, it will be null, 
            // and the method will return false as expected.
            if (!(allowedRegion?.Contains(target) ?? true)
                || (disallowedRegion?.Contains(target) ?? false))
            {
                moved = null;
                replaced = null;
                return false;
            }

            replaced = baseMap[target];
            
            // Fail the move if the target position has an entity which cannot be replaced.
            if (!(replaced?.CanBeReplaced ?? true))
            {
                moved = null;
                replaced = null;
                return false;
            }

            moved = baseMap[source];
            if (moved == null)
            {
                replaced = null;
                return false;
            }

            baseMap[source] = null;
            baseMap[target] = moved;

            // Update the replaced.
            replaced?.SetMap(null);
            replaced?.WasRemoved(target);

            // Update the moved.
            moved.SetPosition(target);
            moved.WasMoved(source);

            return true;
        }

        public bool Move(HexPoint source, HexPoint target, out MapEntity moved)
        {
            MapEntity _;
            return Move(source, target, out moved, out _);
        }

        public bool Move(HexPoint source, HexPoint target)
        {
            MapEntity _, __;
            return Move(source, target, out _, out __);
        }

        public bool Remove(HexPoint position, out MapEntity removed)
        {
            // No need to check if position is in the allowed region.

            removed = baseMap[position];
            baseMap[position] = null;

            removed?.SetMap(null);
            removed?.WasRemoved(position);

            return removed != null;
        }

        public bool Remove(HexPoint position)
        {
            MapEntity _;
            return Remove(position, out _);
        }

        public void Clear()
        {
            foreach (var cell in baseMap)
            {
                cell.value.SetMap(null);
                cell.value.WasRemoved(cell.index);
            }

            baseMap.Clear();
        }
    }
}

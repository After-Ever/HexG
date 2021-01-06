using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class EntityHexMap<T> : IReadOnlyHexMap<EntityHexMapHandle<T>> where T : class
    {
        IHexMap<EntityHexMapHandle<T>> baseMap;
        public readonly IRegion allowedRegion;
        public readonly IRegion disallowedRegion;
        
        /// <summary>
        /// Create a new, empty <see cref="EntityHexMap"/>.
        /// </summary>
        /// <param name="baseMap">The base map that will be used to store the <see cref="T"/>.
        /// Must be empty.</param>
        /// <param name="allowedRegion">If provided, entities can only inhabit positions within this region.</param>
        /// <param name="disallowedRegion">If provided, entities cannot inhabit positions within this region.</param>
        public EntityHexMap(IHexMap<T> baseMap, IRegion allowedRegion = null, IRegion disallowedRegion = null)
        {
            this.baseMap = baseMap?.Map((cell) => new EntityHexMapHandle<T>(this, cell.value, cell.index)) ?? throw new ArgumentNullException();
            this.allowedRegion = allowedRegion;
            this.disallowedRegion = disallowedRegion;
            
            if (!baseMap.IsEmpty)
                throw new Exception("Base map must be empty!");
        }

        public EntityHexMapHandle<T> Add(T entity, HexPoint position, out T replaced)
        {
            if (entity == null)
                throw new ArgumentNullException();
            
            if (!(allowedRegion?.Contains(position) ?? true) 
                || (disallowedRegion?.Contains(position) ?? false))
            {
                replaced = null;
                return null;
            }

            replaced = baseMap[position]?.Data;
            IEntityHexMapEntity<T> replacedEntity = null;

            if (replaced != null 
                && replaced is IEntityHexMapEntity<T> re
                && !re.CanBeReplaced)
            {
                replacedEntity = re;
                replaced = null;
                return null;
            }

            var newHandle = baseMap[position] = new EntityHexMapHandle<T>(this, entity, position);

            replacedEntity?.Removed(position);
            
            if (entity is IEntityHexMapEntity<T> e)
            {
                e.Added(newHandle);
            }

            return newHandle;
        }

        public EntityHexMapHandle<T> Add(T entity, HexPoint position)
        {
            T _;
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
        public bool Move(HexPoint source, HexPoint target, out EntityHexMapHandle<T> moved, out T replaced)
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

            replaced = baseMap[target]?.Data;
            IEntityHexMapEntity<T> replacedEntity = null;

            if (replaced != null
                && replaced is IEntityHexMapEntity<T> re
                && !re.CanBeReplaced)
            {
                replacedEntity = re;
                replaced = null;
                moved = null;
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

            replacedEntity?.Removed(target);

            if (moved.Data is IEntityHexMapEntity<T> e)
            {
                e.Moved(source);
            }

            return true;
        }

        public bool Move(HexPoint source, HexPoint target, out EntityHexMapHandle<T> moved)
        {
            T _;

            return Move(source, target, out moved, out _);
        }

        public bool Move(HexPoint source, HexPoint target)
        {
            EntityHexMapHandle<T> _;
            T __;

            return Move(source, target, out _, out __);
        }

        public bool Remove(HexPoint position, out T removed)
        {
            // No need to check if position is in the allowed region.

            removed = baseMap[position]?.Data;
            baseMap[position] = null;

            if (removed is IEntityHexMapEntity<T> e)
                e.Removed(position);

            return removed != null;
        }

        public bool Remove(HexPoint position)
        {
            T _;
            return Remove(position, out _);
        }

        public void Clear()
        {
            foreach (var cell in baseMap)
            {
                if (cell.value.Data is IEntityHexMapEntity<T> e)
                    e.Removed(cell.index);
            }

            baseMap.Clear();
        }


        // IReadOnlyHexMap:

        public bool IsEmpty => baseMap.IsEmpty;

        EntityHexMapHandle<T> IReadOnlyHexMap<EntityHexMapHandle<T>>.this[HexPoint index]
            => baseMap[index];

        public IEnumerable<Cell<EntityHexMapHandle<T>>> CellsWhere(HexPredicate<EntityHexMapHandle<T>> predicate)
            => baseMap.CellsWhere(predicate);

        public IEnumerable<Cell<EntityHexMapHandle<T>>> CellsWhere(HexPredicate<EntityHexMapHandle<T>> predicate, IEnumerable<HexPoint> indices)
            => baseMap.CellsWhere(predicate, indices);

        public IEnumerable<Cell<EntityHexMapHandle<T>>> CellsWhere(HexPredicate<EntityHexMapHandle<T>> predicate, IRegion searchRegion)
            => baseMap.CellsWhere(predicate, searchRegion);

        public IHexMap<EntityHexMapHandle<T>> Where(HexPredicate<EntityHexMapHandle<T>> predicate)
            => baseMap.Where(predicate);

        public IHexMap<K> Map<K>(Converter<Cell<EntityHexMapHandle<T>>, K> converter) where K : class
            => baseMap.Map(converter);

        public IHexMap<EntityHexMapHandle<T>> GetRegion(IRegion region)
            => baseMap.GetRegion(region);

        public Cell<EntityHexMapHandle<T>> FirstWhere(HexPredicate<EntityHexMapHandle<T>> predicate, IEnumerable<HexPoint> indices)
            => baseMap.FirstWhere(predicate, indices);

        public IEnumerator<Cell<EntityHexMapHandle<T>>> GetEnumerator()
            => baseMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}

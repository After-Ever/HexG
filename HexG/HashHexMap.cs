using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class HashHexMap<T> : IHexMap<T> where T : class
    {
        Dictionary<HexPoint, T> map = new Dictionary<HexPoint, T>();

        public T this[HexPoint index]
        {
            get
            {
                T value;
                if (map.TryGetValue(index, out value))
                    return value;
                return null;
            }
            set
            {
                if (value == null)
                {
                    // Passing null should clear the entry, if there is one.
                    map.Remove(index);
                    return;
                }

                map[index] = value;
            }
        }

        public bool IsEmpty => map.Count == 0;

        public IEnumerable<Cell<T>> CellsWhere(HexPredicate<T> predicate)
            => map
            .Select((kvp) => new Cell<T> { index = kvp.Key, value = kvp.Value })
            .Where((cell) => predicate(cell));

        public IEnumerable<Cell<T>> CellsWhere(HexPredicate<T> predicate, IEnumerable<HexPoint> indices)
            => indices
            .Select((i) => new Cell<T> { index = i, value = this[i] })
            .Where((cell) => predicate(cell));

        public IEnumerable<Cell<T>> CellsWhere(HexPredicate<T> predicate, IRegion searchRegion)
            => searchRegion
            .Select((i) => new Cell<T> { index = i, value = this[i] })
            .Where((cell) => predicate(cell));

        public void Clear()
            => map.Clear();

        public Cell<T> FirstWhere(HexPredicate<T> predicate, IEnumerable<HexPoint> indices)
            => indices
            .Select((i) => new Cell<T> { index = i, value = this[i] })
            .First((cell) => predicate(cell));

        public IEnumerable<Cell<T>> CellsInRegion(IReadOnlyRegion region)
        {
            return map
                .Where((kvp) => region.Contains(kvp.Key))
                .Select((kvp) => new Cell<T> { index = kvp.Key, value = kvp.Value });
        }

        public IHexMap<K> Map<K>(Converter<Cell<T>, K> converter) where K : class
        {
            var newMap = new HashHexMap<K>();
            var cellsToAdd = map
                .Select((kvp) => new Cell<T> { index = kvp.Key, value = kvp.Value })
                .Select((cell) => new Cell<K> { index = cell.index, value = converter(cell) });

            newMap.SetCells(cellsToAdd);

            return newMap;
        }

        public void SetCells(IEnumerable<Cell<T>> cells)
        {
            foreach (var cell in cells)
            {
                this[cell.index] = cell.value;
            }
        }

        public void SetCells(IEnumerable<HexPoint> indices, HexGeneator<T> generator)
        {
            foreach (var index in indices)
            {
                this[index] = generator(index);
            }
        }

        public void SetCells(IHexMap<T> map, HexPoint? offset = null, bool setEmpty = true)
        {
            foreach (var cell in map)
            {
                var pos = cell.index + (offset ?? HexPoint.Zero);

                if (!setEmpty && this[pos] == null)
                    continue;

                this[pos] = cell.value;
            }
        }

        public IHexMap<T> Where(HexPredicate<T> predicate)
        {
            var newMap = new HashHexMap<T>();
            var cellsToAdd = map
                .Select((kvp) => new Cell<T> { index = kvp.Key, value = kvp.Value })
                .Where((cell) => predicate(cell));

            newMap.SetCells(cellsToAdd);

            return newMap;
        }


        // IEnumerable:

        public IEnumerator<Cell<T>> GetEnumerator()
            => map
            .Select((kvp) => new Cell<T> { index = kvp.Key, value = kvp.Value })
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IRegion GetRegion()
            => new Region(this.Select(cell => cell.index));
    }
}

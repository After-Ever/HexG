using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public delegate T HexGeneator<T>(HexPoint index);
    public delegate bool HexPredicate<T>(Cell<T> cell);

    public struct Cell<T>
    {
        public HexPoint index;
        public T value;
    }

    public interface IReadOnlyHexMap<T> : IEnumerable<Cell<T>> where T : class
    {
        T this[HexPoint index] { get; }

        bool IsEmpty { get; }

        /// <summary>
        /// Returns all the cells which pass <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<Cell<T>> CellsWhere(HexPredicate<T> predicate);

        /// <summary>
        /// Returns all the cells referenced by <paramref name="indices"/> which pass <paramref name="predicate"/>.
        /// The order returned will match that of <paramref name="indices"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        IEnumerable<Cell<T>> CellsWhere(HexPredicate<T> predicate, IEnumerable<HexPoint> indices);

        /// <summary>
        /// Returns all the cells within <paramref name="searchRegion"/> which pass <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="searchRegion"></param>
        /// <returns></returns>
        IEnumerable<Cell<T>> CellsWhere(HexPredicate<T> predicate, IRegion searchRegion);

        /// <summary>
        /// Return a copy of this map, clearing any cells which don't pass <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IHexMap<T> Where(HexPredicate<T> predicate);

        /// <summary>
        /// Return a copy of this map, applying <paramref name="converter"/> on each cell.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        IHexMap<K> Map<K>(Converter<Cell<T>, K> converter) where K : class;

        /// <summary>
        /// Return a new map with just the cells in <paramref name="region"/>.
        /// </summary>
        /// <returns></returns>
        IHexMap<T> CellsInRegion(IRegion region);

        /// <summary>
        /// Return an <see cref="IRegion"/> representing every non-empty cell.
        /// </summary>
        /// <returns></returns>
        IRegion GetRegion();

        /// <summary>
        /// Return the first cell referenced by <paramref name="indices"/> which passes <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        Cell<T> FirstWhere(HexPredicate<T> predicate, IEnumerable<HexPoint> indices);
    }

    public interface IHexMap<T> : IReadOnlyHexMap<T> where T : class
    {
        new T this[HexPoint index] { get; set; }

        void SetCells(IEnumerable<Cell<T>> cells);

        /// <summary>
        /// Set each cell referenced by <paramref name="indices"/> to the values
        /// produced by <paramref name="generator"/>.
        /// 
        /// If indices are repeated, those which occur later will overwrite earlier values.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="generator"></param>
        void SetCells(IEnumerable<HexPoint> indices, HexGeneator<T> generator);

        /// <summary>
        /// Each non-empty cell in <paramref name="map"/> will set the value of the cell in this map
        /// at the same location plus <paramref name="offset"/>.
        /// </summary>
        /// <param name="map">The map to copy onto this one.</param>
        /// <param name="offset"></param>
        /// <param name="setEmpty">When false, if a cell is empty in this map, it will remain empty.</param>
        void SetCells(IHexMap<T> map, HexPoint? offset = null, bool setEmpty = true);

        /// <summary>
        /// Clear the contents of all cells.
        /// </summary>
        void Clear();
    }
}

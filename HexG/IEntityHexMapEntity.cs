using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public interface IEntityHexMapEntity<T> where T : class
    {
        /// <summary>
        /// Can this entity be replaced durring map operations?
        /// 
        /// If false, move and add operations which target this entity's
        /// position will fail.
        /// If true, these operations will result in this entity being removed.
        /// </summary>
        bool CanBeReplaced { get; }

        /// <summary>
        /// The entity was added to an <see cref="EntityHexMap{T}"/>.
        /// 
        /// For this to be called, either it is the first time being called, or else <see cref="Removed(HexPoint)"/>
        /// has been called since the last time this was called.
        /// </summary>
        /// <param name="handle"></param>
        void Added(EntityHexMapHandle<T> handle);

        /// <summary>
        /// The entity has been moved within its containing <see cref="EntityHexMap{T}"/>.
        /// 
        /// For this to be called <see cref="Added(EntityHexMapHandle{T})"/> must have been called
        /// at least once, and <see cref="Removed(HexPoint)"/> will not have been called since
        /// the last <see cref="Added(EntityHexMapHandle{T})"/>.
        /// </summary>
        /// <param name="lastPosition"></param>
        void Moved(HexPoint lastPosition);

        /// <summary>
        /// The entity has been removed from its containing <see cref="EntityHexMap{T}"/>.
        ///
        /// For this to be called <see cref="Added(EntityHexMapHandle{T})"/> must have been called
        /// at least once, and <see cref="Removed(HexPoint)"/> will not have been called since
        /// the last <see cref="Added(EntityHexMapHandle{T})"/>.
        /// </summary>
        /// <param name="lastPosition"></param>
        void Removed(HexPoint lastPosition);
    }
}

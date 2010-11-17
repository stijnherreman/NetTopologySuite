#region License

/*
 *  The attached / following is part of NetTopologySuite.Coordinates.Simple.
 *  
 *  NetTopologySuite.Coordinates.Simple is free software ? 2009 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/.
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  This work was derived from NetTopologySuite.Coordinates.ManagedBufferedCoordinate
 *  by codekaizen
 *  
 *  Author: Felix Obermaier 2009
 *  
 */

#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.DataStructures;
#if !DOTNET40
using GeoAPI.DataStructures.Collections.Generic;
using C5;
#endif
using GeoAPI.Diagnostics;
using GeoAPI.Geometries;

namespace NetTopologySuite.Coordinates
{
    using ICoordFactory = ICoordinateFactory<Coordinate>;
    using ICoordSequence = ICoordinateSequence<Coordinate>;
    using ICoordSequenceFactory = ICoordinateSequenceFactory<Coordinate>;

    /// <summary>
    /// Delegate to compute the index
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns>Index</returns>
    internal delegate Int32 IndexComputer(Int32 index);
    /// <summary>
    /// An <see cref="ICoordinateSequence{Coordinate}"/>.
    /// </summary>
    public class CoordinateSequence : ICoordSequence
    {
        private static Int32 ForwardIndex(Int32 index)
        {
            return index;
        }

        private Int32 ReverseIndex(Int32 index)
        {
            return _coordinates.Count - (index + 1);
        }

        private List<Coordinate> _coordinates = new List<Coordinate>();
        private readonly CoordinateSequenceFactory _factory;
        //private Int32 _startIndex, _endIndex;
        private Coordinate _max = new Coordinate();
        private Coordinate _min = new Coordinate();
        private IExtents<Coordinate> _extents;

        /**
         * Constructs a sequence based on the given array (the
         * array is not copied).
         *
         * @param coordinates the coordinate array that will be referenced.
         */

        internal CoordinateSequence(CoordinateSequenceFactory factory)
        {
            _factory = factory;
        }

        //private CoordinateSequence(Boolean reversed, List<Coordinate> coordinates,
        //                           Int32 startIndex, Int32 endIndex,
        //                           CoordinateSequenceFactory factory)
        //{
        //    _coordinates = coordinates;
        //    _reversed = reversed;
        //    _startIndex = startIndex;
        //    _endIndex = endIndex;
        //    _factory = factory;
        //}
        internal CoordinateSequence(CoordinateSequenceFactory factory, IEnumerable<Coordinate> coordinates)
            : this(factory)
        {
            _coordinates.AddRange(Check(coordinates));
        }

        /**
         * Constructs a sequence of a given size, populated
         * with new {@link Coordinate}s.
         *
         * @param size the size of the sequence to create
         */

        internal CoordinateSequence(CoordinateSequenceFactory factory, int size)
            : this(factory)
        {
            _factory = factory;
            _coordinates = new List<Coordinate>(size);
            for (int i = 0; i < size; i++)
            {
                _coordinates.Add(new Coordinate());
            }
        }

        /**
         * Constructs a sequence based on the given array (the
         * array is not copied).
         *
         * @param coordinates the coordinate array that will be referenced.
         */

        internal CoordinateSequence(CoordinateSequence coordSeq)
        {
            if (coordSeq != null)
            {
                _factory = coordSeq._factory;
                foreach (Coordinate coord in coordSeq)
                    _coordinates.Add(Coordinate.Clone(coord));
            }
        }

        #region ICoordinateSequence<Coordinate> Members

        public CoordinateDimensions Dimension
        {
            get { return CoordinateDimensions.Three; }
        }

        /**
         * Get the Coordinate with index i.
         *
         * @param i
         *                  the index of the coordinate
         * @return the requested Coordinate instance
         */

        ICoordinate ICoordinateSequence.this[Int32 i]
        {
            get { return _coordinates[i]; }
            set
            {
                Assert.IsTrue(!value.IsEmpty);
                _coordinates[i] = (Coordinate) value;
            }
        }

        public ICoordinateSequence<Coordinate> Clone()
        {
            return new CoordinateSequence(_factory, _coordinates);
        }

        /**
         * Returns the size of the coordinate sequence
         *
         * @return the number of coordinates
         */

        public Int32 Count
        {
            get { return _coordinates.Count; }
        }

        #endregion

        /**
         * Get a copy of the Coordinate with index i.
         *
         * @param i  the index of the coordinate
         * @return a copy of the requested Coordinate
         */

        public Coordinate CoordinateCopy(int i)
        {
            return Coordinate.Clone(_coordinates[i]);
        }

        /////**
        //// * @see com.vividsolutions.jts.geom.CoordinateSequence#getX(int)
        //// */
        //public void getCoordinate(int index, Coordinate coord) {
        //  coord.x = coordinates[index].x;
        //  coord.y = coordinates[index].y;
        //}

        /**
         * @see com.vividsolutions.jts.geom.CoordinateSequence#getX(int)
         */

        public Double GetX(int index)
        {
            return _coordinates[index].X;
        }

        /**
         * @see com.vividsolutions.jts.geom.CoordinateSequence#getY(int)
         */

        public Double GetY(int index)
        {
            return _coordinates[index].Y;
        }

        /**
         * @see com.vividsolutions.jts.geom.CoordinateSequence#getOrdinate(int, int)
         */

        public Double GetOrdinate(int index, Ordinates ordinateIndex)
        {
            switch (ordinateIndex)
            {
                case Ordinates.X:
                    return _coordinates[index].X;
                case Ordinates.Y:
                    return _coordinates[index].Y;
                case Ordinates.Z:
                    return _coordinates[index].Z;
            }
            return Double.NaN;
        }

        /**
         * Creates a deep copy of the Object
         *
         * @return The deep copy
         */

        /**
         * @see com.vividsolutions.jts.geom.CoordinateSequence#setOrdinate(int, int, double)
         */

        public void SetOrdinate(int index, Ordinates ordinateIndex, double value)
        {
            throw new NotSupportedException("SetOrdinate");
            //switch (ordinateIndex) {
            //  case Ordinates.X:
            //  _coordinates[index].X = value;
            //  break;
            //  case Ordinates.Y:
            //  _coordinates[index].Y = value;
            //  break;
            //  case Ordinates.Z:
            //  _coordinates[index].Z = value;
            //  break;
            //default:
            //    throw new ArgumentOutOfRangeException("invalid ordinateIndex");
            //}
        }

        /**
         * This method exposes the internal Array of Coordinate Objects
         *
         * @return the Coordinate[] array.
         */

        public Coordinate[] CoordinateArray()
        {
            return _coordinates.ToArray();
        }

        public IExtents<Coordinate> ExpandToInclude(IExtents<Coordinate> extents)
        {
            foreach (Coordinate coord in _coordinates)
                extents.ExpandToInclude(coord);
            return extents;
        }

        /**
         * Returns the string Representation of the coordinate array
         *
         * @return The string
         */

        public override String ToString()
        {
            if (Count > 0)
            {
                StringBuilder strBuf = new StringBuilder(17*Count);
                strBuf.Append('(');
                strBuf.Append(_coordinates[0]);
                for (int i = 1; i < Count; i++)
                {
                    strBuf.Append(", ");
                    strBuf.Append(_coordinates[i]);
                }
                strBuf.Append(')');
                return strBuf.ToString();
            }
            return "()";
        }

        private void FindMinMax()
        {
            if (Count < 1)
            {
                return;
            }

            Coordinate maxCoord = new Coordinate();
            Coordinate minCoord = new Coordinate();

            foreach (Coordinate current in _coordinates)
            {
                if (maxCoord.IsEmpty || current.GreaterThan(maxCoord))
                    maxCoord = current;

                if (minCoord.IsEmpty || current.LessThan(minCoord))
                    minCoord = current;
            }

            _max = maxCoord;
            _min = minCoord;
        }

        private Coordinate Check(Coordinate coord)
        {
            if ( coord.IsEmpty )
                throw new InvalidOperationException("Must not add empty coordinate to sequence");

            if ( coord.CoordinateFactory == _factory.CoordinateFactory )
                return coord;

            return _factory.CoordinateFactory.Create(coord);
        }

        private IEnumerable<Coordinate> Check(IEnumerable<Coordinate> coords)
        {
            foreach (Coordinate coord in coords)
                yield return Check(coord);
        }

        #region ICoordSequence<Coordinate> Member

        private Boolean _isFrozen;

        public ICoordinateSequence<Coordinate> AddRange(IEnumerable<Coordinate> coordinates, bool allowRepeated,
                                                        bool reverse)
        {
            CheckFrozen();

            if (reverse)
            {
                Stack<Coordinate> stack = new Stack<Coordinate>(coordinates);
                coordinates = stack;
            }

            if (allowRepeated)
                _coordinates.AddRange(Check(coordinates));
            else
            {
                foreach (Coordinate coordinate in coordinates)
                {
                    if (_coordinates.Count == 0 || !coordinate.Equals(Last))
                        _coordinates.Add(Check(coordinate));
                }
            }

            OnSequenceChanged();
            return this;
        }

        public ICoordinateSequence<Coordinate> AddRange(IEnumerable<Coordinate> coordinates)
        {
            return AddRange(coordinates, true, false);
        }

        public ICoordinateSequence<Coordinate> AddSequence(ICoordinateSequence<Coordinate> sequence)
        {
            return AddRange(sequence, true,false);
        }

        public ICoordinateSequence<Coordinate> Append(Coordinate coordinate)
        {
            if (coordinate.IsEmpty)
            {
                throw new ArgumentException("Cannot add the empty " +
                                            "coordinate to a sequence.");
            }
            return AddRange(new[] { coordinate });
        }

        public ICoordinateSequence<Coordinate> Append(IEnumerable<Coordinate> coordinates)
        {
            return AddRange(coordinates);
        }

        public ICoordinateSequence<Coordinate> Append(ICoordinateSequence<Coordinate> coordinates)
        {
            return AddRange(coordinates);
        }

        /*
        public ISet<Coordinate> AsSet()
        {
            return new CoordinateSet(this, _factory);
        }
         */

        public ICoordinateSequence<Coordinate> Clear()
        {
            _coordinates.Clear();
            return this;
        }

        public ICoordinateSequence<Coordinate> CloseRing()
        {
            if (!_coordinates[0].Equals(_coordinates[Count - 1]))
                _coordinates.Add(Check(_coordinates[0]));
            return this;
        }

        public ICoordinateSequenceFactory<Coordinate> CoordinateSequenceFactory
        {
            get { return _factory; }
        }

        public ICoordinateFactory<Coordinate> CoordinateFactory
        {
            get { return _factory.CoordinateFactory; }
        }

        int ICoordSequence.Count
        {
            get { return Count; }
        }

        public bool Equals(ICoordinateSequence<Coordinate> other, Tolerance tolerance)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (Count != other.Count)
            {
                return false;
            }

            Int32 count = Count;

            for (Int32 index = 0; index < count; index++)
            {
                if (this[index].Equals(other[index]))
                    continue;

                if (!tolerance.Equal(0, this[index].Distance(other[index])))
                    return false;
            }

            return true;
        }

        public IExtents<Coordinate> ExpandExtents(IExtents<Coordinate> extents)
        {
            IExtents<Coordinate> expanded = extents;
            expanded.ExpandToInclude(this);
            return expanded;
        }

        public IExtents<Coordinate> GetExtents(IGeometryFactory<Coordinate> geometryFactory)
        {
            if (_extents == null)
            {
                _extents = geometryFactory.CreateExtents(Minimum, Maximum);
            }

            return _extents;
        }

        public Coordinate First
        {
            get { return Count > 0 ? this[0] : new Coordinate(); }
        }

        public ICoordinateSequence<Coordinate> Freeze()
        {
            _isFrozen = true;
            return this;
        }

        public bool HasRepeatedCoordinates
        {
            get
            {
                Coordinate last = new Coordinate();
                foreach (Coordinate coordinate in _coordinates)
                {
                    if (last.IsEmpty)
                    {
                        last = coordinate;
                        continue;
                    }

                    if (coordinate.Equals(last))
                        return true;
                    last = coordinate;
                }
                return false;
            }
        }

        public int IncreasingDirection
        {
            get
            {
                Int32 midPoint = Count / 2;

                for (Int32 index = 0; index < midPoint; index++)
                {
                    Int32 j = Count - 1 - index;

                    // reflecting about midpoint, compare the coordinates
                    Int32 comp = this[j].CompareTo(this[index]);

                    if (comp != 0)
                    {
                        return comp;
                    }
                }

                // must be a palindrome - defined to be in positive direction
                return 1;
            }
        }

        public ICoordinateSequence<Coordinate> Insert(int index, Coordinate item)
        {
            CheckFrozen();
            _coordinates.Insert(index, Check(item));
            return this;
        }

        public bool IsReadOnly
        {
            get { return _isFrozen; }
        }

        private Int32 computeIndex( Int32 index )
        {
            return !_reversed ? index : LastIndex - index;
        }
        public Coordinate this[int index]
        {
            get { return _coordinates[_indexComputer(index)]; }
            set { _coordinates[_indexComputer(index)] = value; }
        }

        public Coordinate Last
        {
            get { return Count > 0 ? _coordinates[LastIndex] : new Coordinate(); }
        }

        public Coordinate Maximum
        {
            get
            {
                if (_max.IsEmpty) FindMinMax();
                return _max;
            }
        }

        public ICoordinateSequence<Coordinate> Merge(ICoordinateSequence<Coordinate> other)
        {
            ICoordinateSequence<Coordinate> seq = _factory.Create(this);
            return seq.Append(other);
        }

        public Coordinate Minimum
        {
            get
            {
                if (_min.IsEmpty) FindMinMax();
                return _min;
            }
        }

        public ICoordinateSequence<Coordinate> Prepend(Coordinate coordinate)
        {
            return Prepend(new[] {coordinate});
        }

        public ICoordinateSequence<Coordinate> Prepend(IEnumerable<Coordinate> coordinates)
        {
            CheckFrozen();
            if (!_reversed)
            {
                List<Coordinate> newCoords = new List<Coordinate>(Check(coordinates));
                newCoords.AddRange(_coordinates);
                _coordinates = newCoords;
            }
            else
                _coordinates.AddRange(new Stack<Coordinate>(Check(coordinates)));

            OnSequenceChanged();
            return this;
        }

        public ICoordinateSequence<Coordinate> Prepend(ICoordinateSequence<Coordinate> coordinates)
        {
            return Prepend((IEnumerable<Coordinate>)coordinates);
        }

        public ICoordinateSequence<Coordinate> RemoveAt(int index)
        {
            if (_reversed)
                index = LastIndex - index;
            _coordinates.RemoveAt(index);
            OnSequenceChanged();
            return this;
        }

        private Boolean _reversed;
        private IndexComputer _indexComputer = ForwardIndex;
        public ICoordinateSequence<Coordinate> Reverse()
        {
            CheckFrozen();
            if (_reversed)
                _indexComputer = ForwardIndex;
            else
            {
                _indexComputer = ReverseIndex;
            }
            _reversed = !_reversed;
            OnSequenceChanged();
            return this;
        }

        public ICoordinateSequence<Coordinate> Reversed
        {
            get
            {
                ICoordSequence reversed = Clone();
                reversed.Reverse();
                return reversed;
            }
        }

        public ICoordinateSequence<Coordinate> Scroll(Coordinate coordinateToBecomeFirst)
        {
            Int32 index = IndexOf(coordinateToBecomeFirst);

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("coordinateToBecomeFirst",
                                                      coordinateToBecomeFirst,
                                                      "Coordinate not found.");
            }

            return Scroll(index);
        }

        private Boolean IsClosed
        {
            get
            {
                if (_coordinates.Count == 0)
                    return false;
                return First.Equals(Last);
            }
        }

        public ICoordinateSequence<Coordinate> Scroll(int indexToBecomeFirst)
        {
            CheckFrozen();
            Boolean wasClosed = false;
            Int32 count = Count;

            if (IsClosed)
            {
                RemoveAt(LastIndex);
                wasClosed = true;
            }

            List<Coordinate> newCoordinates = new List<Coordinate>(count);
            newCoordinates.AddRange(_coordinates.GetRange(indexToBecomeFirst, Count - indexToBecomeFirst));
            newCoordinates.AddRange(_coordinates.GetRange(0, indexToBecomeFirst));
            _coordinates = newCoordinates;

            if (wasClosed)
                _coordinates.Add(First);

            OnSequenceChanged();

            return this;
        }

        public ICoordinateSequence<Coordinate> Slice(int startIndex, int endIndex)
        {
            return new CoordinateSequence(_factory, GetRange(startIndex, endIndex - startIndex + 1));
        }

        public ICoordinateSequence<Coordinate> Sort()
        {
            return Sort(0, LastIndex, _factory.DefaultComparer);
        }

        public ICoordinateSequence<Coordinate> Sort(int startIndex, int endIndex)
        {
            return Sort(startIndex, endIndex, _factory.DefaultComparer);
        }

        private Boolean IsSlice
        {
            get { return false; }
        }

        public ICoordinateSequence<Coordinate> Sort(int startIndex, int endIndex, IComparer<Coordinate> comparer)
        {
            if ( IsSlice )
            {
                throw new NotSupportedException("Sorting a slice is not supported.");
            }

            CheckFrozen();

            if (startIndex == endIndex)
            {
                return this;
            }

            List<Coordinate> coords = _coordinates.GetRange(startIndex, endIndex - startIndex + 1);
            coords.Sort(comparer);

            for (Int32 i = startIndex; i <= endIndex; i++)
            {
                this[i] = coords[i - startIndex];
            }

            OnSequenceChanged();
            return this;
        }

        //private CoordinateSequence createSliceInternal(Int32 endIndex, Int32 startIndex)
        //{
        //    Freeze();

        //    return new CoordinateSequence(_reversed,
        //                                  _coordinates,
        //                                  startIndex, endIndex,
        //                                  _factory);
        //}

        public ICoordinateSequence<Coordinate> Splice(IEnumerable<Coordinate> coordinates, int startIndex, int endIndex)
        {
            return Splice(coordinates, startIndex, endIndex, (IEnumerable<Coordinate>)null);
        }

        public ICoordinateSequence<Coordinate> Splice(Coordinate coordinate, int startIndex, int endIndex)
        {
            return Splice(new[]{coordinate}, startIndex, endIndex, (IEnumerable<Coordinate>)null);
        }

        private IEnumerable<Coordinate> GetRange(Int32 startIndex, Int32 count)
        {
            IEnumerable<Coordinate> retVal;
            if (!_reversed)
                retVal = _coordinates.GetRange(startIndex,count);
            else
            {
                retVal = new Stack<Coordinate>(
                    _coordinates.GetRange(LastIndex - startIndex - count + 1, count));
            }
            return retVal;
        }

        public ICoordinateSequence<Coordinate> Splice(Coordinate startCoordinate, int startIndex, int endIndex,
                                                      Coordinate endCoordinate)
        {
            return Splice(new[] {startCoordinate}, startIndex, endIndex, new[] {endCoordinate});
        }

        public ICoordinateSequence<Coordinate> Splice(IEnumerable<Coordinate> startCoordinates, int startIndex,
                                                      int endIndex, Coordinate endCoordinate)
        {
            return Splice(startCoordinates, startIndex, endIndex, new[]{endCoordinate});
        }

        public ICoordinateSequence<Coordinate> Splice(Coordinate startCoordinate, int startIndex, int endIndex,
                                                      IEnumerable<Coordinate> endCoordinates)
        {
            return Splice(new[]{startCoordinate}, startIndex, endIndex, endCoordinates);
        }

        public ICoordinateSequence<Coordinate> Splice(IEnumerable<Coordinate> startCoordinates, int startIndex,
                                                      int endIndex, IEnumerable<Coordinate> endCoordinates)
        {
            CoordinateSequence seq = new CoordinateSequence(_factory, GetRange(startIndex, endIndex-startIndex+1));
            if (startCoordinates != null ) 
                seq.Prepend(startCoordinates);
            if( endCoordinates != null) 
                seq.Append(endCoordinates);
            return seq;
        }

        public ICoordinateSequence<Coordinate> Splice(int startIndex, int endIndex, Coordinate coordinate)
        {
            return Splice((IEnumerable<Coordinate>)null, startIndex, endIndex, new[] { coordinate });
        }

        public ICoordinateSequence<Coordinate> Splice(int startIndex, int endIndex, IEnumerable<Coordinate> coordinates)
        {
            return Splice((IEnumerable<Coordinate>)null, startIndex, endIndex, coordinates);
        }

        public Coordinate[] ToArray()
        {
            return _coordinates.ToArray();
        }

        public ICoordinateSequence<Coordinate> WithoutRepeatedPoints()
        {
            List<Coordinate> coords = new List<Coordinate>();
            Coordinate last = new Coordinate();
            foreach (Coordinate coordinate in _coordinates)
            {
                if( !coordinate.Equals(last))
                    coords.Add(coordinate);
                last = coordinate;
            }
            return new CoordinateSequence(_factory, coords);
        }

        protected void SetSequenceInternal(CoordinateSequence sequence)
        {
            if (sequence.IsFrozen)
            {
                _coordinates = sequence._coordinates;
            }
            else
            {
                _coordinates.Clear();
                _coordinates.AddRange(Check(sequence._coordinates));
            }
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public ICoordinateSequence<Coordinate> WithoutDuplicatePoints()
        {
#if DOTNET40
            HashSet<Coordinate> hs = new HashSet<Coordinate>(this);
            return new CoordinateSequence(_factory, hs);
            //hs.AllowsDuplicates = false;
#else
            C5.HashSet<Coordinate> hs = new C5.HashSet<Coordinate>();
            //hs.AllowsDuplicates = false;
            hs.AddAll(_coordinates);
            return new CoordinateSequence(_factory, hs.UniqueItems());
#endif
        }

        public Pair<Coordinate> SegmentAt(int index)
        {
            if (index < 0 || index >= LastIndex)
            {
                throw new ArgumentOutOfRangeException("index", index,
                                                      "Index must be between 0 and LastIndex - 1");
            }
            return new Pair<Coordinate>(this[index], this[index + 1]);
        }

        #endregion

        #region IList<Coordinate> Member

        public int IndexOf(Coordinate item)
        {
            return _coordinates.IndexOf(item);
        }

        void System.Collections.Generic.IList<Coordinate>.Insert(int index, Coordinate item)
        {
            Insert(index, item);
        }

        void System.Collections.Generic.IList<Coordinate>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        Coordinate System.Collections.Generic.IList<Coordinate>.this[int index]
        {
            get { return this[index]; }
            set { this[index] = value; }
        }

        #endregion

        #region ICollection<Coordinate> Member

        public void Add(Coordinate item)
        {
            Append(item);
        }

        void System.Collections.Generic.ICollection<Coordinate>.Clear()
        {
            Clear();
        }

        public bool Contains(Coordinate item)
        {
            return _coordinates.Contains(item);
        }

        public void CopyTo(Coordinate[] array, int arrayIndex)
        {
            for (Int32 index = 0; index < Count; index++)
            {
                array[index + arrayIndex] = this[index];
            }
        }

        int System.Collections.Generic.ICollection<Coordinate>.Count
        {
            get { return Count; }
        }

        public bool Remove(Coordinate item)
        {
            if ( _coordinates.Contains(item))
                return _coordinates.Remove(item);
            return false;
        }

        #endregion

        #region IEnumerable<Coordinate> Member

        public IEnumerator<Coordinate> GetEnumerator()
        {
            IEnumerator<Coordinate> it;
            if (!_reversed)
                return _coordinates.GetEnumerator();

            Stack<Coordinate> stack = new Stack<Coordinate>(_coordinates);
            return stack.GetEnumerator();
        }

        #endregion

        #region IEnumerable Member

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IComparable<ICoordinateSequence<Coordinate>> Member

        public int CompareTo(ICoordinateSequence<Coordinate> other)
        {
            Int32 size1 = Count;
            Int32 size2 = other.Count;

            Int32 dim1 = (Int32) Dimension;
            Int32 dim2 = (Int32) other.Dimension;

            // lower dimension is less than higher
            if (dim1 < dim2) return -1;
            if (dim1 > dim2) return 1;

            // lexicographic ordering of point sequences
            Int32 i = 0;

            while (i < size1 && i < size2)
            {
                Int32 ptComp = this[i].CompareTo(other[i]);

                if (ptComp != 0)
                {
                    return ptComp;
                }

                i++;
            }

            if (i < size1) return 1;
            if (i < size2) return -1;

            return 0;
        }

        #endregion

        #region IEquatable<ICoordinateSequence<Coordinate>> Member

        public bool Equals(ICoordinateSequence<Coordinate> other)
        {
            return Equals(other, Tolerance.Zero);
        }

        #endregion

        #region ICoordinateSequence Member

        Double ICoordinateSequence.this[int index, Ordinates ordinate]
        {
            get { return this[index][ordinate]; }
            set { throw new NotSupportedException(); }
        }

        ICoordinate[] ICoordinateSequence.ToArray()
        {
            ICoordinate[] array = new ICoordinate[Count];

            for (Int32 i = 0; i < Count; i++)
            {
                ICoordinate coord = _coordinates[i];
                array[i] = coord;
            }

            return array;
        }

        public IExtents ExpandExtents(IExtents extents)
        {
            return ExpandExtents((IExtents<Coordinate>) extents);
        }

        ICoordinateSequence ICoordinateSequence.Merge(ICoordinateSequence other)
        {
            throw new NotImplementedException();
        }

        ICoordinateSequence ICoordinateSequence.Clone()
        {
            return Clone();
        }

        public bool Equals(ICoordinateSequence other, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public IExtents GetExtents(IGeometryFactory geometryFactory)
        {
            throw new NotImplementedException();
        }

        ICoordinateSequence ICoordinateSequence.Freeze()
        {
            return Freeze();
        }

        ICoordinate ICoordinateSequence.First
        {
            get { return First; }
        }

        ICoordinate ICoordinateSequence.Last
        {
            get { return Last; }
        }

        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        public int LastIndex
        {
            get { return _coordinates.Count - 1; }
        }

        ICoordinate ICoordinateSequence.Maximum
        {
            get { throw new NotImplementedException(); }
        }

        ICoordinate ICoordinateSequence.Minimum
        {
            get { throw new NotImplementedException(); }
        }

        ICoordinateSequence ICoordinateSequence.Reverse()
        {
            return Reversed;
        }

        ICoordinateSequence ICoordinateSequence.Reversed
        {
            get { return Reverse(); }
        }

        ICoordinateSequenceFactory ICoordinateSequence.CoordinateSequenceFactory
        {
            get { return _factory; }
        }

        Pair<ICoordinate> ICoordinateSequence.SegmentAt(int index)
        {
            throw new NotImplementedException();
        }

        public event EventHandler SequenceChanged;

        #endregion

        #region IList Member

        public int Add(object value)
        {
            if (value is Coordinate)
            {
                Append((Coordinate) value);
                return LastIndex;
            }

            if (value is ICoordinate)
            {
                Append(CoordinateFactory.Create(value as ICoordinate));
                return LastIndex;
            }

            return 0;
        }

        void IList.Clear()
        {
            _coordinates.Clear();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get
            {
                return _isFrozen;
            }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection Member

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { return Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICloneable Member

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        protected void OnSequenceChanged()
        {
            EventHandler e = SequenceChanged;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }

            _min = new Coordinate();
            _max = new Coordinate();
            _extents = null;
        }

        private void CheckFrozen()
        {
            if (_isFrozen)
            {
                throw new InvalidOperationException(
                    "Sequence is frozen and cannot be modified.");
            }
        }

    }
}
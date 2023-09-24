using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the dense array.<br/>
    /// It stores items in the internal array contiguously.<br/>
    /// <i>Dense array have no checks.</i>
    /// </summary>
    /// <typeparam name="T">Type of the array's items.</typeparam>
    public sealed class Dense<T>
    {
        private T[] _array;

        /// <summary>
        /// Count of containing items in the array.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Size of the array.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Constructs a dense array with the specified <see cref="Size"/>.
        /// </summary>
        public Dense(int size = 32)
        {
            _array = new T[size];
            Count = 0;
            Size = size;
        }

        /// <summary>
        /// Adds the specified item to the end of the internal array.<br/>
        /// If the array has not enough <see cref="Size"/>, then extends it twice.
        /// </summary>
        public void Add(T item)
        {
            if (Size == Count)
                DoubleSize();
            _array[Count] = item;
            Count++;
        }

        /// <summary>
        /// Swaps the last internal item with the specified by index one.
        /// </summary>
        public void Remove(int index)
        {
            Count--;
            (_array[index], _array[Count]) = (_array[Count], _array[index]);
        }

        public ref T this[int index] => ref _array[index];

        public Span<T> AsSpan()
        {
            return new Span<T>(_array, 0, Count);
        }

        public Span<T> AsRawSpan()
        {
            return new Span<T>(_array);
        }

        private void DoubleSize()
        {
            Size *= 2;
            Array.Resize(ref _array, Size);
        }
    }
}
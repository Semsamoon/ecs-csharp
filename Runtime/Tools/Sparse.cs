using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the sparse array.<br/>
    /// It stores items in the internal array.<br/>
    /// <i>Sparse array have no checks.</i>
    /// </summary>
    /// <typeparam name="T">Type of the array's items.</typeparam>
    public sealed class Sparse<T>
    {
        private T[] _array;

        /// <summary>
        /// Size of the array.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Constructs a sparse array with the specified <see cref="Size"/>.
        /// </summary>
        public Sparse(int size = 32)
        {
            _array = new T[size];
            Size = size;
        }

        /// <summary>
        /// Adds the specified item to the specified index of the internal array.<br/>
        /// If the array has not enough <see cref="Size"/>, then extends it to fit the index.
        /// </summary>
        public void Add(T item, int index)
        {
            EnsureSize(index + 1);
            _array[index] = item;
        }

        /// <summary>
        /// Removes the item from the specified index of the internal array and places the specified replace item instead of the removed one.
        /// </summary>
        public void Remove(int index, T replace = default)
        {
            _array[index] = replace;
        }

        public ref T this[int index] => ref _array[index];

        public Span<T> AsSpan()
        {
            return new Span<T>(_array);
        }

        private void EnsureSize(int size)
        {
            if (Size >= size)
                return;
            while (size > Size)
                Size *= 2;
            Array.Resize(ref _array, Size);
        }
    }
}
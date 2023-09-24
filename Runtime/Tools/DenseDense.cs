using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the dense array of the dense arrays.<br/>
    /// It stores dense arrays in the internal array contiguously.<br/>
    /// <i>Dense of dense array have no checks.</i>
    /// </summary>
    /// <typeparam name="T">Type of the array's items.</typeparam>
    public sealed class DenseDense<T>
    {
        private Dense<T>[] _array;

        /// <summary>
        /// Count of containing items in the array.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Size of the array.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Size of the default internal array.
        /// </summary>
        public int InternalSize { get; }

        /// <summary>
        /// Constructs a dense of dense array with the specified <see cref="SizeDenseDense"/>.
        /// </summary>
        public DenseDense(SizeDenseDense size = default)
        {
            if (size.Outer == 0)
                size = SizeDenseDense.Default;
            _array = new Dense<T>[size.Outer];
            for (var i = 0; i < size.Outer; i++)
                _array[i] = new Dense<T>(size.Inner);
            Count = 0;
            Size = size.Outer;
            InternalSize = size.Inner;
        }

        /// <summary>
        /// Adds a new internal dense array to the end of the array.<br/>
        /// If the array has not enough <see cref="Size"/>, then extends it twice.
        /// </summary>
        public void Add()
        {
            if (Size == Count)
                DoubleSize();
            Count++;
        }

        /// <summary>
        /// Swaps the last internal dense array with the specified by index one.
        /// </summary>
        public void Remove(int index)
        {
            Count--;
            (_array[index], _array[Count]) = (_array[Count], _array[index]);
        }

        public ref Dense<T> this[int index] => ref _array[index];

        public Span<Dense<T>> AsSpan()
        {
            return new Span<Dense<T>>(_array, 0, Count);
        }

        public Span<Dense<T>> AsRawSpan()
        {
            return new Span<Dense<T>>(_array);
        }

        private void DoubleSize()
        {
            var previousSize = Size;
            Size *= 2;
            Array.Resize(ref _array, Size);
            for (var i = previousSize; i < Size; i++)
                _array[i] = new Dense<T>(InternalSize);
        }
    }

    /// <summary>
    /// Struct of the sizes for the sparse set.<br/>
    /// It contains external and internal sizes.
    /// </summary>
    public readonly struct SizeDenseDense
    {
        /// <summary>
        /// Size for the external dense array.
        /// </summary>
        public int Outer { get; }

        /// <summary>
        /// Size for the internal dense array.
        /// </summary>
        public int Inner { get; }

        /// <summary>
        /// Default value for the size dense of dense.
        /// </summary>
        public static readonly SizeDenseDense Default = new(32, 8);

        /// <summary>
        /// Constructs a size dense of dense with specified <see cref="Outer"/> and <see cref="Inner"/> sizes.
        /// </summary>
        public SizeDenseDense(int outer, int inner)
        {
            Outer = outer;
            Inner = inner;
        }
    }
}
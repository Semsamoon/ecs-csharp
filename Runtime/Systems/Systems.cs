using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the system container.<br/>
    /// <i>It is not recommended to manage systems without using the container.</i>
    /// </summary>
    public sealed class Systems
    {
        private readonly Dense<IInitializeAble> _initializeAble;
        private readonly Dense<IStartAble> _startAble;
        private readonly Dense<IUpdateAble> _updateAble;

        /// <summary>
        /// Array of the initialize able systems.
        /// </summary>
        public ReadOnlySpan<IInitializeAble> InitializeAble => _initializeAble.AsSpan();

        /// <summary>
        /// Array of the start able systems.
        /// </summary>
        public ReadOnlySpan<IStartAble> StartAble => _startAble.AsSpan();

        /// <summary>
        /// Array of the update able systems.
        /// </summary>
        public ReadOnlySpan<IUpdateAble> UpdateAble => _updateAble.AsSpan();

        /// <summary>
        /// Constructs a system container with specified sizes for internal arrays.
        /// </summary>
        public Systems(SizeSystems size = default)
        {
            if (size.Initialize == 0)
                size = SizeSystems.Default;
            _initializeAble = new Dense<IInitializeAble>(size.Initialize);
            _startAble = new Dense<IStartAble>(size.Start);
            _updateAble = new Dense<IUpdateAble>(size.Update);
        }

        /// <summary>
        /// Creates and adds a system of the specified type.
        /// </summary>
        public Systems Add<T>() where T : class, new()
        {
            var system = new T();
            if (system is IInitializeAble initialize)
                _initializeAble.Add(initialize);
            if (system is IStartAble start)
                _startAble.Add(start);
            if (system is IUpdateAble update)
                _updateAble.Add(update);
            return this;
        }

        /// <summary>
        /// Adds the system.
        /// </summary>
        public Systems Add<T>(T system) where T : class
        {
            if (system is IInitializeAble initialize)
                _initializeAble.Add(initialize);
            if (system is IStartAble start)
                _startAble.Add(start);
            if (system is IUpdateAble update)
                _updateAble.Add(update);
            return this;
        }

        /// <summary>
        /// Initializes all the <see cref="InitializeAble"/> systems.
        /// </summary>
        public void Initialize(World world)
        {
            foreach (var system in _initializeAble.AsSpan())
                system.Initialize(world);
        }

        /// <summary>
        /// Starts all the <see cref="StartAble"/> systems.
        /// </summary>
        public void Start()
        {
            foreach (var system in _startAble.AsSpan())
                system.Start();
        }

        /// <summary>
        /// Updates all the <see cref="UpdateAble"/> systems.
        /// </summary>
        public void Update()
        {
            foreach (var system in _updateAble.AsSpan())
                system.Update();
        }
    }

    /// <summary>
    /// Interface of the initialize able system.
    /// </summary>
    public interface IInitializeAble
    {
        /// <summary>
        /// Initializes the system with the specified world.
        /// </summary>
        void Initialize(World world);
    }

    /// <summary>
    /// Interface of the start able system.
    /// </summary>
    public interface IStartAble
    {
        /// <summary>
        /// Starts the system.
        /// </summary>
        void Start();
    }

    /// <summary>
    /// Interface of the update able system.
    /// </summary>
    public interface IUpdateAble
    {
        /// <summary>
        /// Updates the system.
        /// </summary>
        void Update();
    }

    /// <summary>
    /// Struct of the sizes for the system container.<br/>
    /// It contains initialize, start and update sizes.
    /// </summary>
    public readonly struct SizeSystems
    {
        /// <summary>
        /// Size for the initialize able array.
        /// </summary>
        public int Initialize { get; }

        /// <summary>
        /// Size for the start able array.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Size for the update able array.
        /// </summary>
        public int Update { get; }

        /// <summary>
        /// Default value for the size systems.
        /// </summary>
        public static readonly SizeSystems Default = new(16, 16, 16);

        /// <summary>
        /// Constructs a size systems with specified <see cref="Initialize"/>, <see cref="Start"/> and <see cref="Update"/> sizes.
        /// </summary>
        public SizeSystems(int initialize, int start, int update)
        {
            Initialize = initialize;
            Start = start;
            Update = update;
        }
    }
}
namespace SemsamECS
{
    /// <summary>
    /// Struct of the entity.<br/>
    /// It is represented by an identifier <see cref="Id"/> and generation number <see cref="Gen"/>.
    /// </summary>
    public readonly struct Entity
    {
        /// <summary>
        /// Identifier of the entity.<br/>
        /// There cannot be more than one entity with the same identifier at the same time.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Generation number of the entity.<br/>
        /// It allows recycling the entity after it has been removed and using the id again.
        /// </summary>
        public int Gen { get; }

        /// <summary>
        /// Constructs an entity with specified <see cref="Id"/> and <see cref="Gen"/>.
        /// </summary>
        public Entity(int id, int gen)
        {
            Id = id;
            Gen = gen;
        }

        public static bool operator ==(Entity l, Entity r)
        {
            return l.Id == r.Id && l.Gen == r.Gen && l.Id != 0;
        }

        public static bool operator !=(Entity l, Entity r)
        {
            return !(l == r);
        }

        public override string ToString()
        {
            return Id == 0 ? "NULL" : $"({Id.ToString()}; {Gen.ToString()})";
        }
    }
}
using System.Text;

namespace Chemicals
{
    /// <summary>
    /// The Element class is used to classify the <seealso cref="AtomNode"/> class
    /// </summary>
    public class Element
    {
        /// <summary>
        /// Atomic number of the <seealso cref="Element"/>
        /// </summary>
        public readonly int Number;
        /// <summary>
        /// Symbol of the <seealso cref="Element"/>
        /// </summary>
        public readonly string Symbol;
        /// <summary>
        /// Name of the <seealso cref="Element"/>
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Atomic mass of the <seealso cref="Element"/>
        /// </summary>
        public readonly float Mass;
        /// <summary>
        /// Colour representation of the Element
        /// </summary>
        public readonly string Colour;
        /// <summary>
        /// Valency, used to calculate implicit Hydrogens
        /// </summary>
        public readonly int Valency;

        /// <summary>
        /// Creates a new <seealso cref="Element"/>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        /// <param name="mass"></param>
        /// <param name="colour"></param>
        internal Element(int number, string symbol, string name, float mass, string colour, int valency = 0)
        {
            Number = number;
            Symbol = symbol;
            Name = name;
            Mass = mass;
            Colour = colour;
            Valency = valency;
        }

        public string DataPrint()
        {
            var sb = new StringBuilder();
            sb.Append($"Number is {Number}\n");
            sb.Append($"Symbol is {Symbol}\n");
            sb.Append($"Mass is {Mass}\n");
            sb.Append($"Name is {Name}");
            return sb.ToString();
        }

    }
}

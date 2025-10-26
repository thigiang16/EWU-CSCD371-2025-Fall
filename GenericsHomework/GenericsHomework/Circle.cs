namespace GenericsHomework
{
    /// <summary>
    /// Represents a single circle in a Venn diagramm holding a collection of items of the same reference type
    /// </summary>
    public class Circle<T> where T : class
    {
        private readonly HashSet<T> _items = new(); //HashSet<T> automatically prevents duplicate items
        public string Name { get; } //Read-only that stores the name of the circle
        public Circle(string name) => Name = name; //Initializes a new Circle with a given name
        public void Add(T item) => _items.Add(item); //Adds an item to the circle
        public bool Contains(T item) => _items.Contains(item); //Checks whether the given items exists in the circle
        public IEnumerable<T> GetItems() => _items; //Returns all items in the circle (read-only)
        public override string ToString() => $"{Name}: {string.Join(", ", _items)}";
    
    }
}

using System.Runtime.Intrinsics.Arm;

namespace GenericsHomework
{
    public class Node<T>
    {
        public T Value { get; }
        public Node<T> Next { get; private set; }

        public Node(T value) 
        {
            Value = value;
            Next = this;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }

        public void Append(T value)
        {
            Node<T> current = this;
            do
            {
                if (Equals(current.Value, value))
                    throw new InvalidOperationException("Duplicate value detected");
                current = current.Next;
            }while (current != this);

            var newNode = new Node<T>(value) { Next = this.Next };
            this.Next = newNode;
        }

         
    }
}

using System;

namespace GenericsHomework;

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
            return Value?.ToString() ?? string.Empty;
        }

        public void Append(T value)
        {
            Node<T> current = this;
            do
            {
                if (object.Equals(current.Value, value))
                    throw new InvalidOperationException("Duplicate value detected");
                current = current.Next;
            }while (current != this);

            var newNode = new Node<T>(value) { Next = this.Next };
            this.Next = newNode;
        }

        /// <summary>
        /// Removes all nodes from the circular list except the current node.
        /// After this call the current node will point to itself.
        /// </summary>

        public void Clear()
        {
            if (this.Next == this)
                return;

            Node<T> firstRemoved = this.Next;

            Node<T> lastRemoved = firstRemoved;
            while (lastRemoved.Next != this)
            {
                lastRemoved = lastRemoved.Next;
            }

            lastRemoved.Next = firstRemoved;

            this.Next = this;
        }

        /// <summary>
        /// Returns true if the provided value exists in the circular list (compares with object.Equals).
        /// </summary>
        public bool Exists(T value)
        {
            Node<T> current = this;
            do
            {
                if (object.Equals(current.Value, value))
                    return true;
                current = current.Next;
            } while (current != this);

            return false;
        }
    }


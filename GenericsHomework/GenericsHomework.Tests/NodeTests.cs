using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenericsHomework;

namespace GenericsHomework.Tests
{
    [TestClass]
    public sealed class NodeTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeValueandSelfReference()
        {
            var node = new Node<int>(5);
            Assert.AreEqual<int>(5, node.Value);
            Assert.AreEqual<Node<int>>(node,node.Next);
        }

        [TestMethod]
        public void ToString_ValueGiven_ReturnsValueAsString()
        {
            var node = new Node<string>("Hello");
            Assert.AreEqual<string>("Hello", node.ToString());
        }

        [TestMethod]
        public void Append_ValidValue_NodeAppendedAfterCurrent()
        {
            var node1 = new Node<int>(1);
            node1.Append(2);
            var node2 = node1.Next;


            Assert.AreEqual<int>(2, node2.Value);
            Assert.AreEqual<Node<int>>(node1, node2.Next);

        }

      
    }
}

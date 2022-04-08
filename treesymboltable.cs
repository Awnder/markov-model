using System;
using System.Collections;
using System.Collections.Generic;

namespace TreeSymbolTable
{
    internal class Node<K,V> where K : IComparable<K>
    {
        public K key;
        public V value;
        public Node<K,V> L;
        public Node<K,V> R;
        public int count;

        public Node(K key, V value = default)
        {
            this.key = key;
            this.value = value;
            L = null;
            R = null;
            count = 1;
        }

        public int CompareTo(Node<K,V> other)
        {
            return this.key.CompareTo(other.key);
        }

        public override string ToString()
        {
            string lchild = "null";
            string rchild = "null";
            if (L != null)
            {
                lchild = $"{L.key}";
            }
            if (R != null)
            {
                rchild = $"{R.key}";
            }
            return $"{lchild} <- {key}:{value} -> {rchild}";
        } 
    }

    public class TreeSymbolTable<K,V> : IEnumerable<K> where K : IComparable<K>
    {
        private Node<K,V> root;

        public TreeSymbolTable()
        {
            root = null;
        }

        public int Count
        {
            get
            {
                if (root == null)
                {
                    return 0;
                } else {
                    return root.count;
                }
            }
        }

        public V this[K key]
        {
            get
            {  
                return GetValue(key);
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(K key, V value)
        {
            root = Add(key, value, root);
        }

        private Node<K,V> Add(K key, V value, Node<K,V> root)
        {
            if (root == null)
            {
                return new Node<K,V>(key, value);
            }

            int compare = key.CompareTo(root.key); // -1 is <, 0 is ==, +1 is >
            if (compare == -1)
            {
                root.L = Add(key, value, root.L);
            } else if (compare == 1) {
                root.R = Add(key, value, root.R);
            } else {
                throw new ArgumentException($"Error: node with key {key} already exists in symbol table"); 
            }

            root.count++;
            return root;
        }

        public bool ContainsKey(K key)
        {
            if (WalkToNode(key, root) == null)
            {
                return false;
            } else {
                return true;
            }
        }

        public V GetValue(K key)
        {
            return GetValue(key, root);
        }

        private V GetValue(K key, Node<K,V> root)
        {
            if (root == null)
            {
                return default;
            } else {
                if (key.CompareTo(root.key) == -1)
                {
                    return GetValue(key, root.L);
                } else if (key.CompareTo(root.key) == 1) {
                    return GetValue(key, root.R);
                } else {
                    return root.value;
                }
            }
        }

        public K Max()
        {
            return Max(root);
        }

        private K Max(Node<K,V> root)
        {
            if (root == null)
            {
                return default;
            } else {
                if (root.R != null)
                {
                    return Max(root.R);
                }
            }
            return root.key;
        }

        public K Min()
        {
            return Min(root);
        }

        private K Min(Node<K,V> root)
        {
            if (root == null)
            {
                return default;
            } else {
                if (root.L != null)
                {
                    return Min(root.L);
                }
            }
            return root.key;
        }

        private Node<K,V> WalkToNode(K key, Node<K,V> root)
        {
            if (root == null)
            {
                return null;
            }
            int compare = key.CompareTo(root.key);
            if (compare == -1)
            {
                return WalkToNode(key, root.L);
            } else if (compare == 1) {
                return WalkToNode(key, root.R);
            } else {
                return root;
            }
        }

        public K Predecessor(K key)
        {
            Node<K,V> current = WalkToNode(key, root);
            if (current == null)
            {
                throw new ArgumentException($"Error: key {key} does not exist in symbol table");
            }
            if (current.L == null)
            {
                throw new InvalidOperationException($"Error: key {key} does not have a predecessor");
            }
            return Max(current.L);
        }

        public K Successor(K key)
        {
            Node<K,V> current = WalkToNode(key, root);
            if (current == null)
            {
                throw new ArgumentException($"Error: key {key} does not exist in symbol table");
            }
            if (current.R == null)
            {
                throw new InvalidOperationException($"Error: key {key} does not have a predecessor");
            }
            return Max(current.R);
        }

        // public void Remove(K key)
        // {
        //     Remove(key, root);
        // }

        // private Node<K,V> Remove(K key, Node<K,V> root)
        // {
        //     //KEEP TRACK OF COUNTS
        //     if (root.L == null && root.R == null) //node w/ no children
        //     {
        //         return null;
        //     }

        //     // node has one child
        //     if (root.L == null && root.R != null)
        //     {
        //         return root.R;
        //     }
        //     if (root.L != null && root.R == null)
        //     {
        //         return root.L;
        //     }

        //     if (root.L != null & root.R != null) //node has two children
        //     {
        //         Node<K,V> predecessorNode = WalkToNode(Predecessor(root.key));
        //         root.key = predecessorNode.key;
        //         root.value = predecessorNode.value;

        //         Remove(predecessorNode);
        //     }
        //     return root;
            
        // }

        public void Clear()
        {
            root = null;
        }

        public void PrintInOrder()
        {
            PrintInOrder(root);
        }

        private void PrintInOrder(Node<K,V> root)
        {
            if (root != null)
            {
                PrintInOrder(root.L);
                Console.WriteLine(root.key);
                PrintInOrder(root.R);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<K> GetEnumerator()
        {
            foreach (K key in GetEnumerator(root))
            {
                yield return key;
            }
        }

        private IEnumerable<K> GetEnumerator(Node<K,V> root)
        {
            if (root != null)
            {
                foreach(K key in GetEnumerator(root.L))
                {
                    yield return key;
                }
                yield return root.key;
                foreach(K key in GetEnumerator(root.R))
                {
                    yield return key;
                }
            }
        }
    }
}

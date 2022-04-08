using System;
using System.Collections;
using System.Collections.Generic;

namespace ListSymbolTable
{
    internal class Node<K,V>
    {
        public K key;
        public V value;
        public Node<K,V> next;

        public Node(K key, V value = default)
        {
            this.key = key;
            this.value = value;
            next = null;
        }

        public override string ToString()
        {
            return $"{key}:{value}";
        } 
    }

    public class ListSymbolTable<K,V> : IEnumerable<K>
    {
        private Node<K,V> head;
        private int count;

        public int Count
        {
            get {return count;}
        }

        public ListSymbolTable()
        {
            head = null; 
            count = 0; 
        }

        public V this[K key]
        {
            get
            {            
                Node<K,V> current = WalkToNode(key);
                if (current == null)
                {
                    throw new KeyNotFoundException($"Error: key {key} not in symbol table.");
                }
                return current.value;
            }
            set
            {
                Node<K,V> current = WalkToNode(key);
                if (current == null)
                {
                    Add(key, value);
                } else {
                    current.value = value;
                }
            }
        }

        public void Add(K key, V value)
        {
            Node<K,V> node = WalkToNode(key);
            if (node == null)
            {
                Node<K,V> newNode = new Node<K,V>(key, value);
                newNode.next = head;
                head = newNode;
                count++;
            } else {
                throw new ArgumentException($"Error: key {key} already in table");
            }
            
        }

        public void Remove(K key)
        {
            if (count > 0 && head.key.Equals(key)) //make sure not empty list and head key is search key
            {
                head.key = default;
                head.value = default;
                head = head.next;
                count--;
                return; //no need to go to while loop
            }

            Node<K,V> prev = head;
            while (prev != null || prev.next != null)
            {
                if (prev.next.key.Equals(key)) //if node in front has search key
                {
                    Node<K,V> toRemove = prev.next;
                    prev.next = toRemove.next;
                    
                    //explicitly elimintates toRemove for garbage 
                    toRemove.key = default;
                    toRemove.value = default;
                    toRemove.next = null;

                    count--;
                    return;
                }
            }
        }

        public void Clear()
        {
            head = null;
            count = 0;
        }

        private Node<K,V> WalkToNode(int index)
        {
            if (index < 0 || index > count) { throw new IndexOutOfRangeException(); }
            Node<K,V> current = head;
            for (int i = 0; i < index; i++) 
            { 
                current = current.next; 
            }
            return current;
        }

        private Node<K,V> WalkToNode(K key)
        {
            Node<K,V> current = head;
            while (current != null)
            {
                if (key.Equals(current.key))
                {
                    return current;
                }
                current = current.next;
            }
            return null; //if node not found
        }

        public bool Contains(K key)
        {
            Node<K,V> current = WalkToNode(key);
            if (current == null)
            {
                return false;
            }
            return true;
        } 

        public override string ToString()
        {
            return $"DataType: [ListSymbolTable] with {count} nodes.";
        }

        public IEnumerator<K> GetEnumerator()
        {
            Node<K,V> current = head;
            while (current != null)
            {
                yield return current.key;
                current = current.next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

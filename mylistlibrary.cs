using System;
using System.Collections;
using System.Collections.Generic;

namespace MyListLibrary
{
    internal class Node<T> where T : IComparable<T>
    {
        public T data;
        public Node<T> next;

        public Node()
        {
            data = default;
            next = null;
        }

        public Node(T initialVal)
        {
            data = initialVal;
            next = null;
        }

        public int CompareTo(Object obj)
        {
            Node<T> lhs = this;
            Node<T> rhs = obj as Node<T>;
            
            return lhs.data.CompareTo(rhs.data);
        }

    }

    public class MyList<T> : IEnumerable<T> where T : IComparable<T>
    {
        private Node<T> head;
        private Node<T> tail;
        private int count;

        public int Count
        {
            get {return count;}
        }

        public MyList()
        {
            head = null; 
            tail = null;
            count = 0; 
        }

        public MyList(T[] sourceArray)
        {
            count = sourceArray.Length;
            head = new Node<T>(sourceArray[0]); //initialize first node
            tail = head; //only one element in list so make head = tail

            for (int i = 1; i < count; i++) //start at i = 1 b/c dont want to repeat head
            {
                //keep making new tails while incrementing through array, similar to Add(T item)
                Node<T> oldTail = tail;
                tail = new Node<T>(sourceArray[i]);
                oldTail.next = tail;
            }
        }

        public T this[int index]
        {
            get
            {            
                Node<T> current = WalkToIndex(index);
                return current.data;
            }
            set
            {
                Node<T> current = WalkToIndex(index);
                current.data = value;
            }
        }

        public void Add(T item)
        {
            Insert(item, count);
            // Node<T> oldTail = tail;
            // tail = new Node<T>(item);

            // if (count == 0) //if the queue is empty (a new queue)
            // {
            //     head = tail; //make the head the same as tail because there is only one element
            //     count = 1;
            // } else {
            //     oldTail.next = tail; //if not empty, make the oldTail's pointer point to tail
            //     count++;
            // }
        }

        public void Insert(T item, int index)
        {
            if (index < 0 || index > count)
            {
                throw new IndexOutOfRangeException();
            }
            Node<T> newNode = new Node<T>(item);
            if (index == 0)
            {
                Node<T> temp = head;
                head = newNode;
                head.next = temp;
            } else {
                Node<T> current = WalkToIndex(index-1);
                Node<T> nextNode = current.next;

                current.next = newNode;
                newNode.next = nextNode;
            }            
            count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index > count)
            {
                throw new NullReferenceException();
            }
            if (index == 0)
            {
                Node<T> temp = head.next;
                head = temp;
            } else if (index == count) {
                Node<T> temp = WalkToIndex(index-1); //Node before the desired index
                tail = temp;
                tail.next = null;
            } else {
                Node<T> before = WalkToIndex(index-1); //Node before the desired index
                Node<T> after = before.next.next;
                before.next = after;    
            }
            count--;
        }

        public void Clear()
        {
            head.data = default;
            tail = head;
            head.next = null;
            count = 0;
        }

        private Node<T> WalkToIndex(int index)
        {
            if (index < 0 || index > count)
            {
                throw new IndexOutOfRangeException("Error: index given is out of bounds");
            }
            Node<T> current = head;
            for (int i = 0; i < index; i++)
            {
                current = current.next;
            }
            return current;
        }

        public int IndexOf(T item, int start = 0)
        {
            Node<T> current = WalkToIndex(start);

            for (int i = start; i < count; i++)
            {
                if (current.data.Equals(item))
                {
                    return i;
                }
                current = current.next;
            }
            return -1;
        }

        public bool Contains(T item)
        {
            if (IndexOf(item) >= 0)
            {
                return true;
            }
            return false;
        } 

        public void SimpleSort(bool ascending = true)
        {
            for (Node<T> curr = head; curr != null; curr = curr.next) //steps through list with Node i
            {
                Node<T> min_node = curr; //makes sure we don't go through already-sorted values
                Node<T> temp_node = new Node<T>();
                for (Node<T> inner = curr; inner != null; inner = inner.next)
                {
                    if (ascending)
                    {
                        if (inner.data.CompareTo(min_node.data) == -1)
                        {
                            //swapping data is much easier than swapping the nodes and pointers
                            temp_node.data = min_node.data;
                            min_node.data = inner.data;
                            inner.data = temp_node.data;
                        }
                    } else {
                        if (inner.data.CompareTo(min_node.data) == 1) 
                        {
                            temp_node.data = min_node.data;
                            min_node.data = inner.data;
                            inner.data = temp_node.data;
                        }
                    }
                }
            }
        }

        /// --- MERGE SORT METHODS ---

        public void MergeSort() //overload b/c node (private internal class) is unknown to program.cs - cant call MergeSort(Node<T> start)
        {
            head = MergeSort(head);
        }

        private Node<T> MergeSort(Node<T> start)
        {
            //step 1 - divide
            if (start == null || start.next == null) //if list length 0 or 1
            {
                return start; //base case
            }

            //cuts list in half, left is head, right is begOf2nd
            Node<T> endOfFirst = GetHalfwayPoint(start);
            Node<T> begOfSecond = endOfFirst.next;
            endOfFirst.next = null;

            Node<T> lhs = start;
            Node<T> rhs = begOfSecond;

            lhs = MergeSort(lhs);
            rhs = MergeSort(rhs);

            //step 2 - combine
            lhs = MergeBack(lhs, rhs);
            
            return lhs; //returns the beginning of the list
        }

        private Node<T> MergeBack(Node<T> l, Node<T> r)
        {
            Node<T> merge = null; //pointer to the beginning of reassembled list
            Node<T> mergePointer = null; //pointer that looks at lhs and rhs and adds to reassembled list
    
            //first node
            if (l.data.CompareTo(r.data) == -1)
            {
                merge = l;
                l = l.next;
            } else {
                merge = r;
                r = r.next;
            }
            mergePointer = merge;

            //iterate through all the rest - main loop
            while (l != null && r != null)
            {
                if (l.data.CompareTo(r.data) == -1)
                {
                    mergePointer.next = l;
                    l = l.next;
                } else {
                    mergePointer.next = r;
                    r = r.next;
                }
                mergePointer = mergePointer.next;
            }

            //if there are any nodes left in either side merge them
            while (l != null)
            {
                mergePointer.next = l;
                if (l.next != null) { l = l.next; }
                mergePointer = mergePointer.next;
                if (l.next == null) { break; }
            }
            while (r != null)
            {
                mergePointer.next = r;
                if (r.next != null) { r = r.next; }
                mergePointer = mergePointer.next;
                if (r.next == null) { break; }
            }

            l = merge;
            return l;
        }

        private Node<T> GetHalfwayPoint(Node<T> start = null)
        {
            if (start == null) { start = head; }
            Node<T> slowWalker = start; //moves one space
            Node<T> fastWalker = start.next; //moves two spaces, at the end, slowWalker will be in the middle of the LList

            while (fastWalker != null)
            {
                fastWalker = fastWalker.next;
                if (fastWalker != null)
                {
                    fastWalker = fastWalker.next;
                    slowWalker = slowWalker.next;
                }
            }
            return slowWalker;
        }

        /// --- END MERGE SORT METHODS ---

        /// --- QUICK SORT METHODS ---

        public void QuickSort()
        {
            head = QuickSort(head);
        }

        private Node<T> QuickSort(Node<T> start)
        {
            if (start == null || start.next == null) //if list length 0 or 1
            {
                return start; //base case
            }

            Node<T> lhs = start;
            Node<T> rhs = lhs;
            Node<T> counter = lhs;

            while (rhs != null)
            {
                rhs = rhs.next; //find end of list
            }

            while (counter != null)
            {
                if (counter.data.CompareTo(rhs.data) == 1)
                {
                    SwapData(counter, rhs);

                    lhs = QuickSort(counter);
                }

                if (counter.next != null) { counter = counter.next; }
                
                SwapData(lhs, counter);

                if (counter.next == null) { break; }
            }

            return lhs;
        }

        private void SwapData(Node<T> a, Node<T> b)
        {
            Node<T> temp = new Node<T>();
            temp.data = a.data;
            a.data = b.data;
            b.data = temp.data;
        }

        /// --- END QUICK SORT METHODS ---

        public T[] ToArray()
        {
            Node<T> current = head;
            T[] array = new T[count];
            for(int i = 0; i < count; i++)
            {
                array[i] = current.data;
                current = current.next;
            }
            return array;
        } 
        
        public override string ToString()
        {
            string typeGeneral = head.GetType().ToString();
            string typeSpecific = typeGeneral.Substring(typeGeneral.IndexOf('['));
            return $"LinkedList of type {typeSpecific} with {count} Nodes";
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node<T> current = head;
            while (current != null)
            {
                yield return current.data;
                current = current.next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

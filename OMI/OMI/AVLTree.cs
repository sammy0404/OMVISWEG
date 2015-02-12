using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI
{
    class AVLTree : IDatastructure
    {
        Node _root;

        /*
        public void TEST()
        {            
            TryAdd(new KeyValuePair<int, object>(17, 1));             
            TryAdd(new KeyValuePair<int, object>(6, 1));
            TryAdd(new KeyValuePair<int, object>(14, 1));
            TryAdd(new KeyValuePair<int, object>(8, 1));
            TryAdd(new KeyValuePair<int, object>(12, 1));
            TryAdd(new KeyValuePair<int, object>(2, 1));
            TryAdd(new KeyValuePair<int, object>(10, 1));
            TryAdd(new KeyValuePair<int, object>(9, 1));
            TryAdd(new KeyValuePair<int, object>(15, 1));            
            TryAdd(new KeyValuePair<int, object>(7, 1));
            TryAdd(new KeyValuePair<int, object>(16, 1));
            TryAdd(new KeyValuePair<int, object>(5, 1));
            TryAdd(new KeyValuePair<int, object>(4, 1));
            TryAdd(new KeyValuePair<int, object>(11, 1));
            TryAdd(new KeyValuePair<int, object>(3, 1));
            TryAdd(new KeyValuePair<int, object>(13, 1));
            TryAdd(new KeyValuePair<int, object>(1, 1));
        }
        */

        public void Build(List<KeyValuePair<int, object>> kvpList)
        {
            foreach (KeyValuePair<int, object> kvp in kvpList)
                TryAdd(kvp);
        }

        private Node getNode(int k)
        {
            Node x = _root;
            while (x != null && k != x.key)
            {
                if (k < x.key)
                    x = x.left;
                else
                    x = x.right;
            }
            return x;
        }

        public KeyValuePair<int, object> Search(int k)
        {
            var x = getNode(k);
            return new KeyValuePair<int, object>(x.key, x.value);
        }

        public bool TryAdd(KeyValuePair<int, object> kvp)
        {
            Node newNode = new Node(null, kvp.Key, kvp.Value);
            Node parentNode = null;
            Node rootNode = _root;

            while (rootNode != null)
            {
                parentNode = rootNode;
                if (newNode.key < rootNode.key)
                    rootNode = rootNode.left;
                else
                    rootNode = rootNode.right;
            }

            newNode.parent = parentNode;
            if (parentNode == null)
            {
                _root = newNode;
                return true;
            }
            else if (newNode.key < parentNode.key)
                parentNode.left = newNode;
            else
                parentNode.right = newNode;

            //AVL
            Node a = newNode;
            while (true)
            {
                if (parentNode.right == a)
                    parentNode.avl--;
                else
                    parentNode.avl++;

                if (parentNode.avl == 2)
                {
                    if (a.avl == 1)
                    {
                        RoteerR(parentNode);
                        parentNode.avl = 0;
                        a.avl = 0;
                        break;
                    }
                    else if (a.avl == -1)
                    {
                        RoteerL(a);
                        RoteerR(parentNode);
                        a.avl = 0;
                        parentNode.avl = 0;
                        a.parent.avl = 0;
                        break;
                    }
                }
                else if (parentNode.avl == -2)
                {
                    if (a.avl == 1)
                    {
                        RoteerR(a);
                        RoteerL(parentNode);
                        a.avl = 0;
                        parentNode.avl = 0;
                        a.parent.avl = 0;
                        break;
                    }
                    else if (a.avl == -1)
                    {
                        RoteerL(parentNode);
                        parentNode.avl = 0;
                        a.avl = 0;
                        break;
                    }
                }

                if (parentNode == _root || parentNode.avl == 0)
                    break;
                else
                {
                    a = parentNode;
                    parentNode = parentNode.parent;
                }
            }
            return true;
        }

        private void RoteerR(Node root)
        {
            Node pivot = root.left;
            root.left = pivot.right;

            if(pivot.right != null)
                pivot.right.parent = root;
            pivot.right = root;

            pivot.parent = root.parent;
            root.parent = pivot;

            if (pivot.parent == null)
                _root = pivot;
            else
            {
                if (pivot.parent.right == root)
                    pivot.parent.right = pivot;
                else
                    pivot.parent.left = pivot;
            }
        }

        private void RoteerL(Node root)
        {
            Node pivot = root.right;
            root.right = pivot.left;

            if(pivot.left != null)
                pivot.left.parent = root;
            pivot.left = root;

            pivot.parent = root.parent;
            root.parent = pivot;

            if (pivot.parent == null)
                _root = pivot;
            else
            {
                if (pivot.parent.right == root)
                    pivot.parent.right = pivot;
                else
                    pivot.parent.left = pivot;
            }
        }


        public bool TryDelete(int k)
        {
            var delNode = getNode(k);
            if (delNode == null)
                return false;
            else
            {
                bool isLeftChild = delNode.parent.left == delNode;
                
                //Move delNode to bottom of tree
                if (delNode.left != null && delNode.right != null)
                {
                    Node y = getMinNode(delNode.right);
                    Node temp = new Node(y.parent, 0, null);
                    temp.left = y.left;
                    temp.right = y.right;

                    // Give Y, delNode's links
                    y.left = delNode.left;
                    delNode.left.parent = y;

                    y.right = delNode.right;
                    delNode.right.parent = y;

                    y.parent = delNode.parent;
                    if (isLeftChild)
                        delNode.parent.left = y;
                    else
                        delNode.parent.right = y;

                    // Give delNode Y's links
                    delNode.left = temp.left;
                    temp.left.parent = delNode;

                    delNode.right = temp.right;
                    temp.right.parent = delNode;

                    delNode.parent = temp.parent;
                    if (temp.parent.left == y)
                        temp.parent.left = delNode;
                    else
                        temp.parent.right = delNode;

                    // Check if y is new root
                    if (y.parent == null)
                        _root = y;
                }

                // Replace delNode with its left subTree
                if (delNode.left != null)
                {
                    delNode.left.parent = delNode.parent;
                    if (delNode.parent != null)
                    {
                        if (isLeftChild)
                            delNode.parent.left = delNode.left;
                        else
                            delNode.parent.right = delNode.left;
                    }
                    else
                        _root = delNode.left;
                }
                // Replace delNode with its right subTree
                else if (delNode.right != null)
                {
                    delNode.right.parent = delNode.parent;
                    if (delNode.parent != null)
                    {
                        if (isLeftChild)
                            delNode.parent.left = delNode.right;
                        else
                            delNode.parent.right = delNode.right;
                    }
                    else
                        _root = delNode.right;
                }
                // Leaf, so remove reference to delNode
                else
                {
                    if (delNode.parent != null)
                    {
                        if (isLeftChild)
                            delNode.parent.left = null;
                        else
                            delNode.parent.right = null;
                    }
                    else
                        _root = null;
                }

                //Fix AVL
                if (delNode.parent != null)
                {                    
                    Node childNode = delNode;
                    Node parentNode = childNode.parent;
                    while(parentNode != null)
                    {
                        if(parentNode.right == childNode)
                            parentNode.avl++;
                        else
                            parentNode.avl--;

                        childNode = parentNode;
                        parentNode = parentNode.parent;

                        //nog meer AVL to come, ya~~y
                    }
                }

                return true;
            }
        }

        public Node getMinNode(Node x)
        {
            while (x.left != null)
                x = x.left;
            return x;
        }

        public KeyValuePair<int, object> GetMin()
        {
            Node x = getMinNode(_root);
            return new KeyValuePair<int,object>(x.key,x.value);
        }

        public Node getMaxNode(Node x)
        {
            while (x.right != null)
                x = x.right;
            return x;
        }

        public KeyValuePair<int, object> GetMax()
        {
            Node x = getMaxNode(_root);
            return new KeyValuePair<int, object>(x.key, x.value);
        }

        public KeyValuePair<int, object> ExtractMax() 
        {
            var min = GetMin();
            TryDelete(min.Key);
            return min;
        }

        public KeyValuePair<int, object> ExtractMin() 
        {
            var max = GetMin();
            TryDelete(max.Key);
            return max;
        }
    }

    class Node
    {
        public Node parent, left, right;
        public int key;
        public object value;
        public int avl;

        public Node(Node parent, int key, object value)
        {
            this.parent = parent;
            this.value = value;
            this.key = key;
        }
    }
}

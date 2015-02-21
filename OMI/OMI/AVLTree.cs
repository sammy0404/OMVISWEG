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
        
        /*public void TEST()
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
        }*/
        

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
            Node childNode = newNode;
            while (true)
            {
                if (parentNode.right == childNode)
                    parentNode.avl--;
                else
                    parentNode.avl++;

                if (AVL(parentNode, childNode))
                    break;

                if (parentNode == _root || parentNode.avl == 0)
                    break;
                else
                {
                    childNode = parentNode;
                    parentNode = parentNode.parent;
                }
            }
            return true;
        }

        private bool AVL(Node parentNode, Node childNode)
        {
            if (parentNode.avl == 2)
            {
                if (childNode.avl == -1)
                {
                    RoteerL(childNode);                   
                    childNode.parent.avl = 0;
                }
                //if (childNode.avl == 0)
               //     childNode.avl = -1;
                RoteerR(parentNode);
                parentNode.avl = 0;
                childNode.avl = 0;
                return true;
            }
            else if (parentNode.avl == -2)
            {
                if (childNode.avl == 1)
                {
                    RoteerR(childNode);
                    childNode.parent.avl = 0;
                }
               // if (childNode.avl == 0)
               //     childNode.avl = 1;
                RoteerL(parentNode);
                parentNode.avl = 0;
                childNode.avl = 0;
                return true;
            }
            return false;
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
            bool isRoot = false;
            bool isLeftChild = false;
            Node y;
            Node temp;

            var delNode = getNode(k);
            if (delNode == null)
                return false;
            else
            {
                if (delNode.parent == null)
                    isRoot = true;
                if (!isRoot)
                    isLeftChild = delNode.parent.left == delNode;

                //Move delNode to bottom of tree
                if (delNode.left != null && delNode.right != null)
                {
                    y = getMinNode(delNode.right);
                    temp = new Node(y.parent, 0, null);
                    temp.left = y.left;
                    temp.right = y.right;

                    // Give Y, delNode's links and avl
                    y.avl = delNode.avl;

                    if (delNode.left != y)
                    {
                        y.left = delNode.left;
                        delNode.left.parent = y;
                    }
                    else
                        y.left = delNode;


                    if (delNode.right != y)
                    {
                        y.right = delNode.right;
                        delNode.right.parent = y;
                    }
                    else
                        y.right = delNode;


                    y.parent = delNode.parent;
                    if (!isRoot)
                    {
                        if (isLeftChild)
                            delNode.parent.left = y;
                        else
                            delNode.parent.right = y;
                    }

                    // Give delNode Y's links
                    delNode.left = temp.left;
                    //temp.left.parent = delNode;

                    delNode.right = temp.right;
                    if (temp.right != null)
                        temp.right.parent = delNode;

                    if (temp.parent == delNode)
                        delNode.parent = y;
                    else
                        delNode.parent = temp.parent;
                    if (temp.parent.left == y)
                        temp.parent.left = delNode;
                    else if (temp.parent.right == y)
                        temp.parent.right = delNode;

                    // Check if y is new root
                    if (y.parent == null)
                        _root = y;
                    isLeftChild = delNode.parent.left == delNode;
                }

                if (!isRoot)
                    isLeftChild = delNode.parent.left == delNode;

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
                /* if (delNode.parent != null)
                 {
                     bool first = true;
                     Node childNode = delNode;
                     Node otherChild = null;
                     Node parentNode = childNode.parent;
                     while(true)
                     {
                         int oldAvl = parentNode.avl;
                         if (parentNode.right == childNode || (first && !isLeftChild))
                         {
                             parentNode.avl++;
                             otherChild = parentNode.left;
                         }
                         else
                         {
                             parentNode.avl--;
                             otherChild = parentNode.right;
                         }

                         var nextparent = parentNode.parent;
                         AVL(parentNode, otherChild);
                             //break;
                         //parentNode.avl = (oldAvl != 0) ? oldAvl : parentNode.avl;

                         if (parentNode == _root || parentNode.avl != 0)
                             break;
                         else
                         {
                            /* if (nextparent == null)
                                 break;
                             if (nextparent.left == parentNode.parent)
                                 childNode = nextparent.left;
                             else if (nextparent.right == parentNode.parent)
                                 childNode = nextparent.right;
                             else 
                             childNode = parentNode;
                             parentNode = parentNode.parent;
                         }
                         first = false;
                     }
                 }*/
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
            var max = GetMax();
            TryDelete(max.Key);
            return max;
        }

        public KeyValuePair<int, object> ExtractMin() 
        {
            var min = GetMin();
            TryDelete(min.Key);
            return min;
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

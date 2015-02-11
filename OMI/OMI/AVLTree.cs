//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OMI
//{
//    class AVLTree : IDatastructure
//    {
//        Node _root;

//        public void TEST()
//        {
//            TryAdd(new KeyValuePair<int, object>(1, 1));
//            TryAdd(new KeyValuePair<int, object>(3, 1));
//            TryAdd(new KeyValuePair<int, object>(2, 1));            
//        }

//        public void Build(List<KeyValuePair<int, object>> kvpList)
//        {
//            foreach (KeyValuePair<int,object> kvp in kvpList)
//                TryAdd(kvp);
//        }

//        public KeyValuePair<int, object> Search(int k)
//        {
//            Node x = _root;
//            while (x != null && k != x.key)
//            {
//                if (k < x.key)
//                    x = x.left;
//                else
//                    x = x.right;
//            }
//            return new KeyValuePair<int,object>(x.key, x.value);
//        }

//        public bool TryAdd(KeyValuePair<int, object> kvp)
//        {
//            Node newNode = new Node(null, kvp.Key, kvp.Value);
//            Node parentNode = null;
//            Node rootNode = _root;

//            while (rootNode != null)
//            {
//                parentNode = rootNode;
//                if (newNode.key < rootNode.key)
//                    rootNode = rootNode.left;
//                else
//                    rootNode = rootNode.right;
//            }

//            newNode.parent = parentNode;
//            if (parentNode == null)
//            {
//                _root = newNode;
//                return true;
//            }
//            else if (newNode.key < parentNode.key)
//                parentNode.left = newNode;
//            else
//                parentNode.right = newNode;           

//            //AVL
//            Node a = newNode;
//            while (true)
//            {
//                if (parentNode.right == a)
//                    parentNode.avl--;
//                else
//                    parentNode.avl++;

//                //int avlL = (parentNode.left != null) ? parentNode.left.avl + 1 : 0;
//                //int avlR = (parentNode.right != null) ? parentNode.right.avl + 1 : 0;

//                //parentNode.avl = avlL - avlR;

//                if (parentNode.avl == 2)
//                {
//                    if (a.avl == 1)
//                    {
//                        RoteerR(parentNode);
//                        parentNode.avl = 0;
//                        a.avl = 0;
//                    }
//                    else if (a.avl == -1)
//                    {
//                        RoteerL(a);
//                        RoteerR(parentNode);
//                        parentNode.avl = 0;
//                        a.avl = 0;
//                        parentNode = parentNode.parent;
//                    }
//                }
//                else if (parentNode.avl == -2)
//                {
//                    if (a.avl == 1)
//                    {
//                        RoteerR(a);
//                        RoteerL(parentNode);
//                        parentNode.avl = 0;
//                        a.avl = 0;
//                        parentNode = parentNode.parent;
//                    }
//                    else if (a.avl == -1)
//                    {
//                        RoteerL(parentNode);
//                        parentNode.avl = 0;
//                        a.avl = 0;
//                    }
//                    else 
//                    {
//                        ;
//                    }
//                }

//                if (parentNode == _root)
//                    break;
//                else
//                {
//                    a = parentNode;
//                    parentNode = parentNode.parent;
//                }
//            }
//            return true;
//        }

//        private void RoteerR(Node root)
//        {
//            Node pivot = root.left;
//            root.left = pivot.right;
//            pivot.right = root;
//            pivot.parent = root.parent;
//            root.parent = pivot;

//            if (pivot.parent == null)
//                _root = pivot;
//            else
//            {
//                if (pivot.parent.right == root)
//                    pivot.parent.right = pivot;
//                else
//                    pivot.parent.left = pivot;
//            }
//        }               

//        private void RoteerL(Node root)
//        {
//            Node pivot = root.right;
//            root.right = pivot.left;
//            pivot.left = root;
//            pivot.parent = root.parent;
//            root.parent = pivot;

//            if (pivot.parent == null)
//                _root = pivot;
//            else
//            {
//                if (pivot.parent.right == root)
//                    pivot.parent.right = pivot;
//                else
//                    pivot.parent.left = pivot;
//            }          
//        }



//        private void RoteerRR(Node root)
//        {
//            Node pivot = root.left;
//            root.left = pivot.right;
//            pivot.right = root;
//            pivot.parent = root.parent;
//            root.parent = pivot;
//            if (pivot.parent == null)
//                root = pivot;                        
//        }

//        private void RoteerLL(Node root)
//        {
//            Node pivot = root.right;
//            root.right = pivot.left;
//            pivot.left = root;
//            pivot.parent = root.parent;
//            root.parent = pivot;
//            if (pivot.parent == null)
//                root = pivot;
//        }
        
//        public object GetMin()
//        {
//            Node x = _root;
//            while (x.left != null)
//                x = x.left;
//            return x.value;
//        }

//        public object GetMax()
//        {
//            Node x = _root;
//            while (x.right != null)
//                x = x.right;
//            return x.value;
//        }

//        public bool TryDelete(int k) { return false; }
//        public object ExtractMax() { return new NotImplementedException(); }
//        public object ExtractMin() { return new NotImplementedException(); }
//    }

//    class Node
//    {
//        public Node parent, left, right;
//        public int key;
//        public object value;
//        public int avl;

//        public Node(Node parent, int key, object value)
//        {
//            this.parent = parent;
//            this.value = value;
//            this.key = key;
//        }
//    }
//}

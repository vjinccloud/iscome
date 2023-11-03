using System;
using System.Collections.Generic;


namespace IscomG2C.Utility
{
    public interface ITreeNode2<T, TKey> : ITreeNode<T>
        where T : class
        where TKey : IComparable
    {

        TKey ID
        {
            get;
        }

        TKey ParentID
        {
            get;
        }
    }

    public interface ITreeNode<T>
        where T : class
    {

        T Parent
        {
            get;
            set;
        }

        IList<T> Children
        {
            get;
            set;
        }
    }

    public class TreeHelper
    {
        public static void BuildParentRelationship<T, TKey>(IEnumerable<T> flatNodeList)
            where T : class, ITreeNode2<T, TKey>
            where TKey : IComparable
        {

            Dictionary<TKey, T> dictionary = new Dictionary<TKey, T>();
            foreach (T node in flatNodeList)
            {
                dictionary.Add(node.ID, node);
            }

            foreach (T node in flatNodeList)
            {
                if (node.ParentID != null && dictionary.ContainsKey(node.ParentID))
                {
                    node.Parent = dictionary[node.ParentID];
                }
            }
        }

        /// <summary>
        /// 查找 Id 跟 parentId, 以設定 ITreeNode2 的 Parent 
        /// 做的事跟 BuildParentRelationship 一樣, 不同的地方是有回傳 Dictionary<TKey, T>        
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="flatNodeList"></param>
        /// <returns></returns>
        public static Dictionary<TKey, T> BuildParentRelationship2<T, TKey>(IEnumerable<T> flatNodeList)
            where T : class, ITreeNode2<T, TKey>
            where TKey : IComparable
        {

            Dictionary<TKey, T> dictionary = new Dictionary<TKey, T>();
            foreach (T node in flatNodeList)
            {
                dictionary.Add(node.ID, node);
            }

            foreach (T node in flatNodeList)
            {
                if (node.ParentID != null && dictionary.ContainsKey(node.ParentID))
                {
                    node.Parent = dictionary[node.ParentID];
                }
            }
            return dictionary;
        }
        public static IList<T> ConvertToForest<T, TKey>(IList<T> flatNodeList)
            where T : class, ITreeNode2<T, TKey>
            where TKey : IComparable
        {

            Dictionary<TKey, T> dictionary = new Dictionary<TKey, T>();
            foreach (T node in flatNodeList)
            {
                dictionary.Add(node.ID, node);
            }

            foreach (T node in flatNodeList)
            {
                if (node.ParentID != null && dictionary.ContainsKey(node.ParentID))
                {
                    node.Parent = dictionary[node.ParentID];
                }
            }

            return ConvertToForest<T>(flatNodeList);
        }

        public static IList<T> ConvertToForest<T>(IEnumerable<T> flatNodeList)
            where T : class, ITreeNode<T>
        {

            Dictionary<T, bool> dictionary = new Dictionary<T, bool>();
            foreach (T node in flatNodeList)
            {
                dictionary.Add(node, true);
                node.Children = new List<T>();
            }

            List<T> rootNodes = new List<T>();

            foreach (T node in flatNodeList)
            {

                if (node.Parent == null)
                { // this is a root node; add it to the rootNodes list.
                    rootNodes.Add(node);
                }
                else
                { // this is not a root node; add it as a child of its parent.

                    if (!dictionary.ContainsKey(node.Parent))
                    {
                        continue;
                    }

                    node.Parent.Children.Add(node);
                }
            }

            return rootNodes;
        }

        public static NodeType GetNodeType<T>(T node)
            where T : class, ITreeNode<T>
        {
            EnsureTreePopulated(node, "node");

            if (node.Parent == null)
            {
                return NodeType.Root;
            }
            else if (node.Children.Count == 0)
            {
                if (node.Parent.Children[node.Parent.Children.Count - 1] == node)
                    return NodeType.LastNode;
                return NodeType.Leaf;
            }
            if (node.Parent.Children[node.Parent.Children.Count - 1] == node)
                return NodeType.LastNode;
            return NodeType.Internal;
        }

        public static bool AreRootNodes<T>(IList<T> nodes) where T : class, ITreeNode<T>
        {
            bool result = true;
            foreach (T t in nodes)
            {
                if (GetNodeType<T>(t) != NodeType.Root)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #region Useful tree properties


        /// <summary>
        /// Checks whether there is a loop from the current node up the tree back to the current node.
        /// It is recommended that this is checked to be false before saving the node to your data store.
        /// </summary>
        /// <example>
        /// The most simple example of a hierarchy loop is were there are 2 nodes, "A" and "B", and "A" 
        /// is "B"'s parent, and "B" is "A"'s parent. This is not allowed, and should not be saved. , 
        /// </example>
        public bool HasHeirachyLoop<T>(T node)
            where T : class, ITreeNode<T>
        {
            EnsureTreePopulated(node, "node");

            T tempParent = node.Parent;
            while (tempParent != null)
            {

                if (tempParent.Equals(node))
                {
                    return true;
                }
                tempParent = tempParent.Parent;
            }
            return false;

        }


        /// <summary>Returns the root node of the tree that the given TreeNode belongs in</summary>
        public T GetRootNode<T>(T node)
            where T : class, ITreeNode<T>
        {
            EnsureTreePopulated(node, "node");

            T cur = node;
            while (cur.Parent != null)
            {
                cur = cur.Parent;
            }
            return cur;
        }


        /// <summary>
        /// Gets the depth of a node, e.g. a root node has depth 0, its children have depth 1, etc.
        /// </summary>
        public int GetDepth<T>(T node)
            where T : class, ITreeNode<T>
        {
            EnsureTreePopulated(node, "node");

            int depth = 0;
            while (node.Parent != null)
            {
                ++depth;
                node = node.Parent;
            }
            return depth;
        }

        #endregion

        #region Search methods

        /// <summary>Finds the TreeNode with the given Id in the given tree by searching the descendents.
        /// Returns null if the node cannot be found.</summary>
        public T FindDescendant<T>(T searchRoot)
            where T : class, ITreeNode<T>
        {
            EnsureTreePopulated(searchRoot, "searchRoot");

            foreach (T child in Iterators.DepthFirstTraversal(searchRoot))
            {
                if (child.Equals(searchRoot))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>Finds the TreeNode with the given id from the given forest of trees.
        /// Returns null if the node cannot be found.</summary>
        public T FindTreeNode<T>(IEnumerable<T> trees)
            where T : class, ITreeNode<T>
        {

            foreach (T rootNode in trees)
            {
                if (rootNode.Equals(rootNode))
                {
                    return rootNode;
                }
                T descendant = FindDescendant(rootNode);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        #endregion

        #region Iterators

        /// <summary>
        /// Provides a variety of iterators to simplify traversal through a tree or forest of trees.
        /// </summary>
        public static class Iterators
        {

            /// <summary>
            /// Returns an Iterator which starts at the given node, and climbs up the tree to
            /// the root node.
            /// </summary>
            /// <param name="startNode">The node to start iterating from.  This will be the first node returned by the iterator.</param>
            public static IEnumerable<T> ClimbToRoot<T>(T startNode)
                where T : class, ITreeNode<T>
            {
                EnsureTreePopulated(startNode, "startNode");

                T current = startNode;
                while (current != null)
                {
                    yield return current;
                    current = current.Parent;
                }
            }

            /// <summary>
            /// Returns an Iterator which starts at the root node, and goes down the tree to
            /// the given node node.
            /// </summary>
            /// <param name="startNode">The node to start iterating from.  This will be the first node returned by the iterator.</param>
            public static List<T> FromRootToNode<T>(T startNode)
                where T : class, ITreeNode<T>
            {
                EnsureTreePopulated(startNode, "node");

                List<T> nodeToRootList = new List<T>();
                foreach (T n in ClimbToRoot(startNode))
                {
                    nodeToRootList.Add(n);
                }
                nodeToRootList.Reverse();
                return nodeToRootList;
            }


            /// <summary>
            /// Returns an Iterator which starts at the given node, and traverses the tree in
            /// a depth-first search manner.
            /// </summary>
            /// <param name="startNode">The node to start iterating from.  This will be the first node returned by the iterator.</param>
            public static IEnumerable<T> DepthFirstTraversal<T>(T startNode)
                where T : class, ITreeNode<T>
            {
                EnsureTreePopulated(startNode, "node");

                yield return startNode;
                foreach (T child in startNode.Children)
                {
                    foreach (T grandChild in DepthFirstTraversal(child))
                    {
                        yield return grandChild;
                    }
                }
            }

            /// <summary>
            /// Returns an Iterator which traverses a forest of trees in a depth-first manner.
            /// </summary>
            /// <param name="trees">The forest of trees to traverse.</param>
            public static IEnumerable<T> DepthFirstTraversal<T>(IEnumerable<T> trees)
                where T : class, ITreeNode<T>
            {
                foreach (T rootNode in trees)
                {
                    foreach (T node in DepthFirstTraversal(rootNode))
                    {
                        yield return node;
                    }
                }
            }


            /// <summary>
            /// Gets the siblings of the given node. Note that the given node is included in the
            /// returned list.  Throws an <see cref="Exception" /> if this is a root node.
            /// </summary>
            /// <param name="node">The node whose siblings are to be returned.</param>
            /// <param name="includeGivenNode">If false, then the supplied node will not be returned in the sibling list.</param>
            public static IEnumerable<T> Siblings<T>(T node, bool includeGivenNode)
                where T : class, ITreeNode<T>
            {
                EnsureTreePopulated(node, "node");

                if (GetNodeType(node) == NodeType.Root)
                {
                    if (includeGivenNode)
                    {
                        yield return node;
                    }
                    yield break;
                }

                foreach (T sibling in node.Parent.Children)
                {
                    if (!includeGivenNode && sibling.Equals(node))
                    { // current node is supplied node; don't return it unless it was asked for.
                        continue;
                    }
                    yield return sibling;
                }

            }


            /// <summary>
            /// Traverses the tree in a breadth-first fashion.
            /// </summary>
            /// <param name="node">The node to start at.</param>
            /// <param name="returnRootNode">If true, the given node will be returned; if false, traversal starts at the node's children.</param>
            public static IEnumerable<T> BreadthFirstTraversal<T>(T node, bool returnRootNode)
                where T : class, ITreeNode<T>
            {
                EnsureTreePopulated(node, "node");

                if (returnRootNode)
                {
                    yield return node;
                }

                foreach (T child in node.Children)
                {
                    yield return child;
                }


                foreach (T child in node.Children)
                {
                    foreach (T grandChild in BreadthFirstTraversal(child, false))
                    {
                        yield return grandChild;
                    }
                }

            }

        }

        #endregion

        #region Private methods

        [System.Diagnostics.Conditional("DEBUG")]
        private static void EnsureTreePopulated<T>(T node, string parameterName)
            where T : class, ITreeNode<T>
        {
            if (node == null)
            {
                throw new ArgumentNullException(parameterName, "The given node cannot be null.");
            }
            if (node.Children == null)
            {
                throw new ArgumentException("The children of " + parameterName + " is null. Have you populated the tree fully by calling TreeHelper<T>.ConvertToForest(IEnumerable<T> flatNodeList)?", parameterName);
            }
        }

        #endregion
    }

    /// <summary>
    /// A type of tree node.
    /// </summary>
    public enum NodeType
    {
        Root, Internal, Leaf, LastNode
    }
}

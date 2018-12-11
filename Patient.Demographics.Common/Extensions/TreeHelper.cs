using System;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Demographics.Common.Extensions
{
    public class TreeHelper
    {
        public static TNode Find<TNode>(IList<TNode> nodes, Func<TNode, bool> matchFound,
            Func<TNode, IList<TNode>> selectChildren)
        {
            var node = default(TNode);
            foreach (var startNode in nodes)
            {
                if (matchFound(startNode))
                {
                    return startNode;
                }
                node = Find<TNode>(selectChildren(startNode), matchFound, selectChildren);
                if (!EqualityComparer<TNode>.Default.Equals(node, default(TNode)))
                    break;
            }
            return node;
        }

        public static IList<TProp> SelectProperties<TNode, TProp>(IList<TNode> nodes, Func<TNode, TProp> propertySelector, Func<TNode, IList<TNode>> childrenSelector)
        {
            var list = new List<TProp>();
            foreach (var node in nodes)
            {
                list.Add(propertySelector(node));
                list.AddRange(SelectProperties(childrenSelector(node), propertySelector, childrenSelector));
            }
            return list;
        }

        public static IList<TNode> BuildTree<TNode, TId>(IList<TNode> nodes, Func<TNode, TId> idSelector, Func<TNode, TId>
            parentIdSelector,
            Action<TNode, TNode> addChild)
        {
            Dictionary<TId, TNode> lookup = new Dictionary<TId, TNode>();

            foreach (var item in nodes)
            {
                var id = idSelector(item);
                if (!lookup.ContainsKey(id))
                {
                    lookup.Add(id, item);
                } 
                //lookup.Add(idSelector(item), item);
            }
            foreach (var item in lookup.Values)
            {
                TNode proposedParent;
                var parentId = parentIdSelector(item);
                if (parentId != null && lookup.TryGetValue(parentId, out proposedParent))
                {
                    addChild(proposedParent, item);
                }
            }

            return lookup.Values.Where(x => parentIdSelector(x) == null).ToList();
        }

        public static void Traverse<TNode>(TNode rootNode, Action<TNode> action, Func<TNode, IList<TNode>> childSelector)
        {
            action(rootNode);
            foreach (var child in childSelector(rootNode))
                Traverse(child, action, childSelector);
        }

        public static IList<TNode> Flatten<TNode>(TNode node, Func<TNode, IList<TNode>> childSelector, Action<TNode> childCleanup)
        {
            //List<TNode> nodes = new List<TNode> { node };
            //var children = childSelector(node).ToList();
            //nodes.AddRange(children);
            //foreach (var node1 in children)
            //{
            //    nodes.AddRange(Flatten(node1, childSelector, childCleanup).ToList());
            //}
            //foreach (var child in children)
            //{
            //    childCleanup(child);
            //}
            //childCleanup(node);
            //return nodes;
           var result= new[] { node }.Union(childSelector(node).SelectMany(x => Flatten(x, childSelector, childCleanup))).ToList();
            /*   childCleanup(node);*/
             return result;
        }

        public static int GetHeight<TNode>(TNode node, Func<TNode, IList<TNode>> childSelector,
            Func<TNode, bool> hasChildren)
        {
            if (!hasChildren(node))
                return 0;
            int maxDepth = 0;
            foreach (TNode n in childSelector(node))
            {
                maxDepth = Math.Max(maxDepth, GetHeight(n, childSelector, hasChildren));
            }

            return maxDepth + 1;
        }

        /// <summary>
        /// Returns maximum number of nodes a tree can contain given a configuration of max nodes per node and a given depth.
        /// N: maximumNumberOfChildrenPerNode
        /// L: depth
        /// (N^L-1) / (N-1).
        /// </summary>
        /// <param name="maximumNumberOfChildrenPerNode">Maximum number of children per node</param>
        /// <param name="depth">Depth of the tree (including root)</param>
        /// <returns>Maximum node count of the tree</returns>
        public static int MaxNodes(int maximumNumberOfChildrenPerNode, int depth)
        {
            if (maximumNumberOfChildrenPerNode == 1)
                return depth;
            if (maximumNumberOfChildrenPerNode < 1)
                return 1;
            return (int)((Math.Pow(maximumNumberOfChildrenPerNode, depth) - 1) / (maximumNumberOfChildrenPerNode - 1));
        }
    }
}
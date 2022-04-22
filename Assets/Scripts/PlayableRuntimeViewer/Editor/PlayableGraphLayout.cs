using System.Collections.Generic;
using UnityEngine;
using YaoJZ.Playable.Node;
using YaoJZ.Playable.PlayableViewer;

namespace YaoJZ.Playable
{
    /// <summary>
    /// https://rachel53461.wordpress.com/2014/04/20/algorithm-for-drawing-trees/
    /// </summary>
    public class PlayableGraphLayout
    {
        private static float nodeWidth = 300;
        private static float nodeHeight = 300;

        private static Dictionary<int, List<GraphNodeViewBase>> _nodesInSameDepth =
            new Dictionary<int, List<GraphNodeViewBase>>();
        
        public static void Layout(PlayableGraphView graphView)
        {
            _nodesInSameDepth.Clear();
            foreach (var rootNode in graphView.Roots)
            {
                LayoutRootNode(rootNode,0);
                AvoidCollision(rootNode);
            }
        }

        private static List<GraphNodeViewBase> GetNodesInSameDepth(int depth)
        {
            if (!_nodesInSameDepth.ContainsKey(depth))
            {
                _nodesInSameDepth.Add(depth,new List<GraphNodeViewBase>());
            }   
            return _nodesInSameDepth[depth];
        }

        private static void AddNodesInDepth(GraphNodeViewBase node)
        {
            var nodes = GetNodesInSameDepth(node.Depth);
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
            }
        }

        private static void AvoidCollision(GraphNodeViewBase node)
        {
            foreach (var childNode in node.Children)
            {
                AvoidCollision(childNode);
            }
            if (!node.IsLeaf && node.SiblingIndex !=0)
            {
                if (CheckChildrenCollision(node,out var maxY))
                {
                    LayoutRootNode(node,maxY + nodeWidth);
                }
            }
        }

        private static bool CheckChildrenCollision(GraphNodeViewBase node,out float maxY)
        {
            var leftSiblingNode = node.Parent.Children[node.SiblingIndex - 1];
            GetTreeMinMaxY(leftSiblingNode,out var minY,out maxY);
            var leftMostChildPos = node.Children[0].Position;
            return leftMostChildPos.y < maxY;
        }
        
        private static void LayoutRootNode(GraphNodeViewBase node,float baseY)
        {
            foreach (var childNode in node.Children)
            {
                LayoutRootNode(childNode,baseY);
            }

            AddNodesInDepth(node);
            float x = -node.Depth * nodeWidth;
            //node.SetPosition(new Rect(x, node.SiblingIndex*nodeHeight,0,0));
            if (node.IsLeaf)
            {
                node.Position = new Vector2(x, node.SiblingIndex * nodeHeight + baseY);
                UpdateNodePosition(node);
                var pos = node.GetPosition();
                //Debug.Log($"{pos.width},{pos.height},{node.name}");
            }
            else
            {
                var y = GetChildrenMiddleY(node);
                node.Position = new Vector2(x, y + baseY);
                UpdateNodePosition(node);
            }
        }

        private static void UpdateNodePosition(GraphNodeViewBase node)
        {
            node.SetPosition(new Rect(node.Position.x,node.Position.y,0,0));
        }

        /// <summary>
        /// 得到子树的最大最小y值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        private static void GetTreeMinMaxY(GraphNodeViewBase node,out float minY,out float maxY)
        {
            minY = float.MaxValue;
            maxY = float.MinValue;
            GetTreeMinMaxYImp(node,ref minY,ref maxY);
        }

        private static void GetTreeMinMaxYImp(GraphNodeViewBase node,ref float minY,ref float maxY)
        {
            foreach (var child in node.Children)
            {
                GetTreeMinMaxYImp(child,ref minY, ref maxY);
            }
            var p = node.Position;
            minY = Mathf.Min(minY, p.y);
            maxY = Mathf.Max(maxY, p.y);
        }

        private static float GetChildrenMiddleY(GraphNodeViewBase node)
        {
            if (node.Children.Count == 1)
            {
                return node.Children[0].Position.y;
            }
            float min = float.MaxValue;
            float max = float.MinValue;
            foreach (var childNode in node.Children)
            {
                //var p = childNode.GetPosition();
                var p = childNode.Position;
                min = Mathf.Min(min, p.y);
                max = Mathf.Max(max, p.y);
            }
            return (max - min) / 2;
        }
    }
}
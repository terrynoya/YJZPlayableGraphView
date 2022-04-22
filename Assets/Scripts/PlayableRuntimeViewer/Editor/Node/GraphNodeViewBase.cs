using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace YaoJZ.Playable.Node
{
    public abstract class GraphNodeViewBase:UnityEditor.Experimental.GraphView.Node
    {
        public int Depth;
        public GraphNodeViewBase Parent;
        public List<GraphNodeViewBase> Children = new List<GraphNodeViewBase>();

        public Vector2 Position;

        public int SiblingIndex
        {
            get
            {
                if (Parent == null)
                {
                    return 0;
                }
                return Parent.Children.IndexOf(this);
            }
        }

        public void AddChild(GraphNodeViewBase child)
        {
            Children.Add(child);
        }

        public bool IsLeaf => Children.Count == 0;
        
        public abstract void UpdateView();
        public abstract Port GetPort(Direction direction, int index);

    }
}
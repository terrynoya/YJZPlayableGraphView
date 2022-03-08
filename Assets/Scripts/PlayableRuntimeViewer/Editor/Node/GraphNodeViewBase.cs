using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace YaoJZ.Playable.Node
{
    public abstract class GraphNodeViewBase:UnityEditor.Experimental.GraphView.Node
    {
        public int Depth;
        public int SiblingIndex;

        public GraphNodeViewBase Parent;

        public List<GraphNodeViewBase> Children = new List<GraphNodeViewBase>();

        public void AddChild(GraphNodeViewBase child)
        {
            Children.Add(child);
        }

        public bool IsLeaf => Children.Count == 0;
        
        public abstract void UpdateView();
        public abstract Port GetPort(Direction direction, int index);

    }
}
using UnityEditor.Experimental.GraphView;

namespace YaoJZ.Playable.Node
{
    public abstract class GraphNodeViewBase:UnityEditor.Experimental.GraphView.Node
    {
        public int Depth;
        public int SiblingIndex;
        public abstract void UpdateView();
        public abstract Port GetPort(Direction direction, int index);

    }
}
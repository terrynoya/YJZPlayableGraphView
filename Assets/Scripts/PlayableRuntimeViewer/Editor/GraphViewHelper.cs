using UnityEditor.Experimental.GraphView;
using YaoJZ.Playable.Node;

namespace PlayableRuntimeViewer.Editor
{
    public class GraphViewHelper
    {
        public static Edge AddEdge(GraphNodeViewBase inputNode, GraphNodeViewBase outputNode, int inputIndex,
            int outputIndex)
        {
            var outputPort = inputNode.GetPort(Direction.Output, inputIndex);
            var inputPort = outputNode.GetPort(Direction.Input, outputIndex);
            Edge edge = new Edge();
            edge.output = outputPort;
            edge.input = inputPort;

            outputPort.Connect(edge);
            inputPort.Connect(edge);

            return edge;
        }
    }
}
//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Animations;
using UnityEngine.Playables;
using YaoJZ.Playable.Node;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace YaoJZ.Playable.PlayableViewer
{
    public class PlayableGraphView:GraphView
    {
        private EditorWindow _editorWindow;
        
        private PlayableGraph _graphData;

        private List<PlayableNodeViewBase> _nodes = new List<PlayableNodeViewBase>();

        public PlayableGraph GraphData
        {
            get { return _graphData; }
            set
            {
                _graphData = value;
                Refresh();
            }
        }

        // private void TestRefresh()
        // {
        //     PlayableGraph graph =PlayableGraph.Create("yjz_graph");
        //     graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        //     
        //     AnimationClipPlayable ap = AnimationClipPlayable.Create(graph,null);
        //     PlayableNodeViewBase n0 = new PlayableNodeViewBase(ap);
        //     var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
        //         typeof(Port));
        //     n0.outputContainer.Add(inputPort);
        //     AddElement(n0);
        //
        //     AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(graph);
        //     PlayableNodeViewBase mix = new AnimationPlayableNodeView(mixer);
        //     AddElement(mix);
        //     var outputPort =
        //         Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
        //     mix.inputContainer.Add(outputPort);
        //     
        //     Edge edge = new Edge();
        //     edge.output = inputPort;
        //     edge.input = outputPort;
        //     edge.input.Connect(edge);
        //     edge.output.Connect(edge);
        //     
        //     AddElement(edge);
        // }

        private void Refresh()
        {
            //this.RemoveAllChildren();
            int outputCount = _graphData.GetOutputCount();
            for (int i = 0; i < outputCount; i++)
            {
                PlayableOutput output =  _graphData.GetOutput(i);
                UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
                CreateChildrenPlayable(playable);
            }
            
            for (int i = 0; i < outputCount; i++)
            {
                PlayableOutput output =  _graphData.GetOutput(i);
                UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
                var node = GetNodeByPlayable(playable);
                for (int j = 0; j < playable.GetInputCount(); j++)
                {
                    var input = playable.GetInput(j);
                    var inputNode = GetNodeByPlayable(input);

                    var outputPort = inputNode.GetPort(Direction.Output, 0);
                    var inputPort = node.GetPort(Direction.Input, j);
                    Edge edge = new Edge();
                    edge.output = outputPort;
                    edge.input = inputPort;

                    outputPort.Connect(edge);
                    inputPort.Connect(edge);
                    
                    AddElement(edge);
                    
                }
            }
        }

        public void UpdateView()
        {
            foreach (var node in _nodes)
            {
                node.UpdateView();
            }
        }

        private PlayableNodeViewBase GetNodeByPlayable(UnityEngine.Playables.Playable playable)
        {
            foreach (var nodeView in _nodes)
            {
                if (nodeView.Playable.Equals(playable))
                {
                    return nodeView;
                }
            }

            return null;
        }

        private void CreateChildrenPlayable(UnityEngine.Playables.Playable playable)
        {
            PlayableNodeViewBase node = null;
            if (playable.GetPlayableType() == typeof(AnimationClipPlayable))
            {
                node = new AnimationPlayableNodeView(playable);
            }
            else
            {
                node = new PlayableNodeViewBase(playable);
            }
            _nodes.Add(node);
            AddElement(node);
            int len = playable.GetInputCount();
            for (int i = 0; i < len; i++)
            {
                var input = playable.GetInput(i);
                CreateChildrenPlayable(input);
            }
        }

        // protected override SearchMenuWindowProvierBase GetSearchWindowProvider()
        // {
        //     return ScriptableObject.CreateInstance<PlayableSearchMenuWindowProvider>();
        // }

        public PlayableGraphView(EditorWindow editorWindow)
        {
            _editorWindow = editorWindow;
            Init();
        }
        
        protected void Init()
        {
            // ノードを追加
            // AddElement(new AnimationClipNode());

            // 親のサイズに合わせてGraphViewのサイズを設定
            this.StretchToParentSize();

            // MMBスクロールでズームインアウトができるように
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            // MMBドラッグで描画範囲を動かせるように
            this.AddManipulator(new ContentDragger());
            // LMBドラッグで選択した要素を動かせるように
            this.AddManipulator(new SelectionDragger());
            // LMBドラッグで範囲選択ができるように
            // this.AddManipulator(new RectangleSelector());
            
            // this.Insert(0,new GridBackground());
            this.Insert(0,new TempGridBackground());
            
            
            //TestRefresh();
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return ports.ToList();
        }
    }
}
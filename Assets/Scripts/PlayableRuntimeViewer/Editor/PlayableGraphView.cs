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
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace YaoJZ.Playable.PlayableViewer
{
    public class PlayableGraphView:GraphView
    {
        private EditorWindow _editorWindow;
        private PlayableGraph _graphData;
        private Queue<UnityEngine.Playables.Playable> _queue =new Queue<UnityEngine.Playables.Playable>();
        private List<PlayableNodeViewBase> _tempDepthNodes = new List<PlayableNodeViewBase>();

        private List<PlayableNodeViewBase> _nodes = new List<PlayableNodeViewBase>();
        private List<Edge> _edges = new List<Edge>();

        private Vector2 _cellSize = new Vector2(250,200);
        
        private Dictionary<int,int> _depthCountMap = new Dictionary<int, int>();
        
        private int _selectedOutputIndex;
        
        public PlayableGraph GraphData
        {
            get { return _graphData; }
            set
            {
                _graphData = value;
                Refresh();
            }
        }

        public int SelectedOutputIndex
        {
            get => _selectedOutputIndex;
            set
            {
                _selectedOutputIndex = value;
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


        private void ClearNodes()
        {
            foreach (var edge in _edges)
            {
                RemoveElement(edge);
            }
            _edges.Clear();
            foreach (var node in _nodes)
            {
                RemoveElement(node);
            }
            _nodes.Clear();
        }

        private void Refresh()
        {
            ClearNodes();
            //this.RemoveAllChildren();
            CreateNodes();
            AddEdges();
            LayoutGraph();
        }

        private void CreateNodes()
        {
            // int outputCount = _graphData.GetOutputCount();
            // for (int i = 0; i < outputCount; i++)
            // {
            //     PlayableOutput output =  _graphData.GetOutput(i);
            //     UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
            //     CreateChildrenPlayable(playable,0);
            // }
            PlayableOutput output =  _graphData.GetOutput(_selectedOutputIndex);
            UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
            CreateChildrenPlayable(playable,0);
        }

        private void AddEdges()
        {
            // int outputCount = _graphData.GetOutputCount();
            // for (int i = 0; i < outputCount; i++)
            // {
            //     PlayableOutput output =  _graphData.GetOutput(i);
            //     UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
            //     AddEdges(playable);
            // }
            PlayableOutput output =  _graphData.GetOutput(_selectedOutputIndex);
            UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
            AddEdges(playable);
        }

        private void LayoutGraph()
        {
            _queue.Clear();
            // int outputCount = _graphData.GetOutputCount();
            // for (int i = 0; i < outputCount; i++)
            // {
            //     PlayableOutput output =  _graphData.GetOutput(i);
            //     UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
            //     _queue.Enqueue(playable);
            // }
            PlayableOutput output =  _graphData.GetOutput(_selectedOutputIndex);
            UnityEngine.Playables.Playable p =  output.GetSourcePlayable();
            _queue.Enqueue(p);
            int depth = -1;
            int index = 0;
            while (_queue.Count > 0)
            {
                var playable = _queue.Dequeue();
                var node = GetNodeByPlayable(playable);
                if (node.Depth != depth)
                {
                    depth = node.Depth;
                    index = 0;
                }
                else
                {
                    index++;
                }

                int nodeCountInDepth = GetDepthCount(node.Depth);
                node.SetPosition(new Rect(-depth*_cellSize.x,index*_cellSize.y - nodeCountInDepth/2f*_cellSize.y,0,0));
                
                int len = playable.GetInputCount();
                for (int i = 0; i < len; i++)
                {
                    _queue.Enqueue(playable.GetInput(i));
                }
            }
        }

        private void AddDepthMap(int depth)
        {
            if (!_depthCountMap.ContainsKey(depth))
            {
                _depthCountMap.Add(depth,1);
            }
            else
            {
                _depthCountMap[depth]++;
            }
        }

        private int GetDepthCount(int depth)
        {
            _depthCountMap.TryGetValue(depth, out var value);
            return value;
        }

        private void AddEdges(UnityEngine.Playables.Playable playable)
        {
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
                _edges.Add(edge);

                AddEdges(input);
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

        private void CreateChildrenPlayable(UnityEngine.Playables.Playable playable,int depth = 0)
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
            node.Depth = depth;
            AddDepthMap(depth);
            _nodes.Add(node);
            AddElement(node);
            int len = playable.GetInputCount();
            for (int i = 0; i < len; i++)
            {
                var input = playable.GetInput(i);
                CreateChildrenPlayable(input,depth+1);
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
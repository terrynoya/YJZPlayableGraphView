//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System.Collections.Generic;
using PlayableRuntimeViewer.Editor;
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
        private readonly List<GraphNodeViewBase> _nodes = new List<GraphNodeViewBase>();
        private readonly List<Edge> _edges = new List<Edge>();

        private int _selectedOutputIndex;

        private readonly List<GraphNodeViewBase> _roots = new List<GraphNodeViewBase>();

        public List<GraphNodeViewBase> Roots => _roots;
        
        public PlayableGraph GraphData
        {
            get => _graphData;
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
            _roots.Clear();
        }

        private void Refresh()
        {
            ClearNodes();
            CreateNodes();
            AddEdges();
            PlayableGraphLayout.Layout(this);
        }

        private void CreateNodes()
        {
            int outputCount = _graphData.GetOutputCount();
            for (int i = 0; i < outputCount; i++)
            {
                PlayableOutput output =  _graphData.GetOutput(i);
                UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
                PlayableOutputNodeView nodeView = new PlayableOutputNodeView(output);
                nodeView.Depth = 0;
                _nodes.Add(nodeView);
                _roots.Add(nodeView);
                AddElement(nodeView);
                CreatePlayableNodeViewRecirsive(nodeView,playable);
            }
            
            // PlayableOutput output =  _graphData.GetOutput(_selectedOutputIndex);
            // UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
            // CreateChildrenPlayable(playable,0);
        }

        private void AddEdges()
        {
            int outputCount = _graphData.GetOutputCount();
            for (int i = 0; i < outputCount; i++)
            {
                PlayableOutput output =  _graphData.GetOutput(i);
                // UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
                // AddEdges(playable);
                AddEdges(output);
            }
            // PlayableOutput output =  _graphData.GetOutput(_selectedOutputIndex);
            // UnityEngine.Playables.Playable playable =  output.GetSourcePlayable();
            // AddEdges(playable);
        }

        private void AddEdges(PlayableOutput output)
        {
            var node = GetNodeView(output);
            var playable = output.GetSourcePlayable();
            var inputNode =GetNodeView(playable);
            var edge = GraphViewHelper.AddEdge(inputNode, node, 0, 0);
            AddElement(edge);
            _edges.Add(edge);
            AddEdges(playable);
        }

        private void AddEdges(UnityEngine.Playables.Playable playable)
        {
            var node = GetNodeView(playable);
            for (int j = 0; j < playable.GetInputCount(); j++)
            {
                var input = playable.GetInput(j);
                var inputNode = GetNodeView(input);
                var edge = GraphViewHelper.AddEdge(inputNode, node, 0, j);
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

        private GraphNodeViewBase GetNodeView<T>(T data)
        {
            foreach (var nodeView in _nodes)
            {
                GraphNodeView<T> node = nodeView as GraphNodeView<T>;
                if (node == null)
                {
                    continue;
                }
                if (node.Data.Equals(data))
                {
                    return nodeView;
                }
            }

            return null;
        }

        private void CreatePlayableNodeViewRecirsive(GraphNodeViewBase parentNodeView,UnityEngine.Playables.Playable playable)
        {
            PlayableNodeView node = null;
            if (playable.GetPlayableType() == typeof(AnimationClipPlayable))
            {
                node = new AnimationPlayableNodeView(playable);
            }
            else
            {
                node = new PlayableNodeView(playable);
            }
            node.Depth = parentNodeView.Depth+1;
            node.Parent = parentNodeView;
            parentNodeView.AddChild(node);
            _nodes.Add(node);
            AddElement(node);
            int len = playable.GetInputCount();
            for (int i = 0; i < len; i++)
            {
                var input = playable.GetInput(i);
                CreatePlayableNodeViewRecirsive(node,input);
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
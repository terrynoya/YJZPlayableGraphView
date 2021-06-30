//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace YaoJZ.Playable.PlayableViewer
{
    public class PlayableGraphViewerEditorWindow:EditorWindow
    {
        private PlayableGraphView _graphView;
        private int _selectedIndex;
        private List<PlayableGraph> _graphDatas = new List<PlayableGraph>();
        private List<string> _displayOptions = new List<string>();
            
        [MenuItem("YJZ/PlayableGraphViewer")]
        public static void Open()
        {
            PlayableGraphViewerEditorWindow window = GetWindow<PlayableGraphViewerEditorWindow>("PlayableGraphViewer");
            window.Init();
        }

        public void SetData(PlayableGraph graphData)
        {
            _graphView.GraphData = graphData;
        }
        
        public void Init()
        {
            _graphView = new PlayableGraphView(this);
            rootVisualElement.Add(_graphView);
            _graphDatas.Clear();
            _graphDatas.AddRange(UnityEditor.Playables.Utility.GetAllGraphs());
            Debug.Log($"graph count:{_graphDatas.Count}");
            if (_graphDatas.Count > 0)
            {
                _graphView.GraphData = _graphDatas[0];
            }
            AddToolBar();
        }
        
        void OnEnable()
        {
            //m_Graphs = new List<PlayableGraph>(UnityEditor.Playables.Utility.GetAllGraphs());

            UnityEditor.Playables.Utility.graphCreated += OnGraphCreated;
            UnityEditor.Playables.Utility.destroyingGraph += OnDestroyingGraph;
        }
        
        void OnDisable()
        {
            UnityEditor.Playables.Utility.graphCreated -= OnGraphCreated;
            UnityEditor.Playables.Utility.destroyingGraph -= OnDestroyingGraph;
        }
        
        void OnGraphCreated(PlayableGraph graph)
        {
            if (!_graphDatas.Contains(graph))
            {
                _graphDatas.Add(graph);
            }
        }

        void OnDestroyingGraph(PlayableGraph graph)
        {
            _graphDatas.Remove(graph);
        }

        private void AddToolBar()
        {
            var toolbar = new IMGUIContainer(OnGuiHandler);
            rootVisualElement.Add(toolbar);
        }

        private void OnGuiHandler()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            _displayOptions.Clear();
            foreach (var graph in _graphDatas)
            {
                string name = graph.GetEditorName();
                _displayOptions.Add(name.Length != 0 ? name : "[Unnamed]");
            }
            var len = _graphDatas.Count;
            if (len > 0 && _selectedIndex<=len-1)
            {
                var selectedGraph = _graphDatas[_selectedIndex];
                EditorGUILayout.Popup(selectedGraph.GetEditorName(), _selectedIndex, _displayOptions.ToArray());
            }
                
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void Update()
        {
            // If in Play mode, refresh the graph each update.
            if (EditorApplication.isPlaying)
            {
                Repaint();
                _graphView?.UpdateView();
            }
        }
    }
}
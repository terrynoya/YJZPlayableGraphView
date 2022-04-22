//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace YaoJZ.Playable.PlayableViewer
{
    public class PlayableGraphViewerEditorWindow:EditorWindow
    {
        private PlayableGraphView _graphView;
        private int _selectedGraphIndex;
        private int _selectedOutputIndex;
        private List<PlayableGraph> _graphDatas = new List<PlayableGraph>();
        private List<string> _graphOptions = new List<string>();
        private List<string> _outputOptions = new List<string>();

        private Button _changeGraphBtn;
        private Button _changeGraphOutputIndexBtn;
        private Button _layoutBtn;
            
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
            _changeGraphBtn = new Button();
            _changeGraphBtn.clicked += OnChangeGraphClick;
            rootVisualElement.Add(_changeGraphBtn);
            
            _changeGraphOutputIndexBtn = new Button();
            _changeGraphOutputIndexBtn.clicked += OnChangeGraphOutputBtnClick;
            rootVisualElement.Add(_changeGraphOutputIndexBtn);

            _layoutBtn = new Button();
            _layoutBtn.text = "layout graph";
            _layoutBtn.clicked += OnLayoutBtnClick;
            rootVisualElement.Add(_layoutBtn);
            
            
            _graphDatas.Clear();
            _graphDatas.AddRange(UnityEditor.Playables.Utility.GetAllGraphs());
            Debug.Log($"graph count:{_graphDatas.Count}");
            
            if (_graphDatas.Count > 0)
            {
                var graphData = _graphDatas[0];
                _graphView.GraphData = graphData;
                _changeGraphBtn.text = graphData.GetEditorName();
                _changeGraphOutputIndexBtn.text = graphData.GetOutput(_selectedOutputIndex).GetEditorName();
            }
            
            AddToolBar();
        }

        private void OnLayoutBtnClick()
        {
            PlayableGraphLayout.Layout(_graphView);
        }

        private void OnChangeGraphOutputBtnClick()
        {
            GenericMenu genericMenu = new GenericMenu();
            var graph = _graphView.GraphData;
            if (!graph.IsValid())
            {
                return;
            }
            var count = graph.GetOutputCount();
            for (int i = 0; i < count; i++)
            {
                int index = i;
                var output = graph.GetOutput(index);
                Debug.Log(output.GetEditorName());
                genericMenu.AddItem(new GUIContent(output.GetEditorName()), false, () =>
                {
                    _graphView.SelectedOutputIndex = index;
                    _changeGraphOutputIndexBtn.text = graph.GetOutput(index).GetEditorName();
                });   
            }
            genericMenu.ShowAsContext();
        }

        private void OnChangeGraphClick()
        {
            GenericMenu genericMenu = new GenericMenu();
            Debug.Log($"graph count:{_graphDatas.Count}");
            for (int i = 0; i < _graphDatas.Count; i++)
            {
                var graph = _graphDatas[i];
                var index = i;
                genericMenu.AddItem(new GUIContent(graph.GetEditorName()), false, () =>
                {
                    _graphView.GraphData = graph;
                    _selectedGraphIndex = index;
                    UpdateGraphChangeBtnText();
                });   
            }
            genericMenu.ShowAsContext();
        }

        private void UpdateGraphChangeBtnText()
        {
            var graph = _graphDatas[_selectedGraphIndex];
            _changeGraphBtn.text = graph.GetEditorName();
        }

        void OnEnable()
        {
            //m_Graphs = new List<PlayableGraph>(UnityEditor.Playables.Utility.GetAllGraphs());
            Utility.graphCreated += OnGraphCreated;
            Utility.destroyingGraph += OnDestroyingGraph;
        }
        
        void OnDisable()
        {
            Utility.graphCreated -= OnGraphCreated;
            Utility.destroyingGraph -= OnDestroyingGraph;
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
            // var toolbar = new IMGUIContainer(OnGuiHandler);
            // rootVisualElement.Add(toolbar);
        }

        private void OnGuiHandler()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            _graphOptions.Clear();
            foreach (var graph in _graphDatas)
            {
                string name = graph.GetEditorName();
                _graphOptions.Add(name.Length != 0 ? name : "[Unnamed]");
            }

            var len = _graphDatas.Count;
            if (len > 0 && _selectedGraphIndex<=len-1)
            {
                var selectedGraph = _graphDatas[_selectedGraphIndex];
                EditorGUILayout.Popup(selectedGraph.GetEditorName(), _selectedGraphIndex, _graphOptions.ToArray());
                
                var outputLen = selectedGraph.GetOutputCount();
                _outputOptions.Clear();
                for (int i = 0; i < outputLen; i++)
                {
                    _outputOptions.Add($"output_{i}");
                }
                if (outputLen > 0)
                {
                    var newIndex = EditorGUILayout.Popup($"output:{_selectedOutputIndex}", _selectedOutputIndex, _outputOptions.ToArray());
                    if (newIndex != _selectedGraphIndex)
                    {
                        _graphView.SelectedOutputIndex = newIndex;
                    }
                    _selectedGraphIndex = newIndex;
                }
                
            }
                
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void Update()
        {
            _graphDatas.Clear();
            _graphDatas.AddRange(Utility.GetAllGraphs());
            if (_graphDatas.Count > 0)
            {
                var currentGraph = _graphDatas[0];
                if (_graphView != null && !currentGraph.Equals(_graphView.GraphData))
                {
                    _graphView.GraphData = currentGraph;
                    UpdateGraphChangeBtnText();
                }
            }
            // If in Play mode, refresh the graph each update.
            if (EditorApplication.isPlaying)
            {
                Repaint();
                _graphView?.UpdateView();
            }
        }
    }
}
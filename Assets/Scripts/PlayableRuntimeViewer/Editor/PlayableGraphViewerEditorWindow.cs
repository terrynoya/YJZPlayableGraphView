//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace YaoJZ.Playable.PlayableViewer
{
    public class PlayableGraphViewerEditorWindow:EditorWindow
    {
        private PlayableGraphView _graphView;

        private List<PlayableGraph> _graphDatas = new List<PlayableGraph>();
        
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
            Debug.Log(_graphDatas.Count.ToString());
            if (_graphDatas.Count > 0)
            {
                _graphView.GraphData = _graphDatas[0];
            }
        }
        
        void Update()
        {
            // If in Play mode, refresh the graph each update.
            if (EditorApplication.isPlaying)
            {
                Repaint();
                _graphView.UpdateView();
            }
        }
    }
}
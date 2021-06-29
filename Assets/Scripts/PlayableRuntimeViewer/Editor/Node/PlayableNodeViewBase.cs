//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace YaoJZ.Playable.Node
{
    public class PlayableNodeViewBase:UnityEditor.Experimental.GraphView.Node
    {
        protected UnityEngine.Playables.Playable _playable;

        private Label _lblInputCount;
        private Label _lblOutputCount;
        private Label _lblPlayState;
        private Label _lblTime;

        private List<Port> _inputs = new List<Port>();
        private List<Port> _outputs = new List<Port>();

        public UnityEngine.Playables.Playable Playable
        {
            get => _playable;
        }
        
        public List<Port> Inputs
        {
            get => _inputs;
        }
        
        public List<Port> Outputs
        {
            get => _outputs;
        }

        public Port GetPort(Direction direction,int index)
        {
            List<Port> ports = direction == Direction.Input ? _inputs : _outputs;
            return ports[index];
        }
        
        public PlayableNodeViewBase(UnityEngine.Playables.Playable playable)
        {
            _playable = playable;
            this.title = _playable.GetPlayableType().Name;
            
            _lblPlayState = new Label();
            mainContainer.Add(_lblPlayState);
            _lblTime = new Label();
            mainContainer.Add(_lblTime);
            _lblInputCount = new Label();
            mainContainer.Add(_lblInputCount);
            _lblOutputCount = new Label();
            mainContainer.Add(_lblOutputCount);

            for (int i = 0; i < _playable.GetInputCount(); i++)
            {
                var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi,
                    typeof(Port));
                port.portName = "input";
                inputContainer.Add(port);
                _inputs.Add(port);
            }
            
            for (int i = 0; i < _playable.GetOutputCount(); i++)
            {
                var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                    typeof(Port));
                port.portName = "output";
                outputContainer.Add(port);
                _outputs.Add(port);
            }
            CreateChildren();
            
            UpdateView();
        }

        protected virtual void CreateChildren()
        {
            
        }

        public void UpdateView()
        {
            _lblPlayState.text = $"PlayState:{_playable.GetPlayState()}";
            _lblTime.text = $"Time:{_playable.GetTime()}";
            _lblInputCount.text = $"InputCount:{_playable.GetInputCount()}";
            _lblOutputCount.text = $"InputCount:{_playable.GetOutputCount()}";
        }
    }
}
//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace YaoJZ.Playable.Node
{
    public class PlayableOutputNodeView:GraphNodeView<PlayableOutput>
    {
        private List<Port> _inputs = new List<Port>();
        private List<Port> _outputs = new List<Port>();
        
        private Label _lblValid;
        
        public PlayableOutputNodeView(PlayableOutput data):base(data)
        {
            titleContainer.style.backgroundColor = GetColor();
            style.color = Color.black;
            
            title = GetContentTypeShortName();
            
            var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi,
                typeof(Port));
            port.portName = "input";
            inputContainer.Add(port);
            _inputs.Add(port);
            
            _lblValid = new Label();
            mainContainer.Add(_lblValid);
        }

        public override void UpdateView()
        {
            title = _data.GetEditorName();
            _lblValid.text = $"IsValid:{_data.IsOutputValid()}";
        }

        public override Port GetPort(Direction direction,int index)
        {
            List<Port> ports = direction == Direction.Input ? _inputs : _outputs;
            return ports[index];
        }
        
        public string GetContentTypeShortName()
        {
            // Remove the extra Playable at the end of the Playable types.
            string shortName = GetContentTypeName().Split('.').Last();
            string cleanName = RemoveFromEnd(shortName, "PlayableOutput") + "Output";
            return string.IsNullOrEmpty(cleanName) ? shortName : cleanName;
        }
        
        protected static string RemoveFromEnd(string str, string suffix)
        {
            if (str.EndsWith(suffix))
            {
                return str.Substring(0, str.Length - suffix.Length);
            }

            return str;
        }
        
        public virtual string GetContentTypeName()
        {
            Type type = _data.GetPlayableOutputType();
            return type == null ? "Null" : type.ToString();
        }
        
        private Color GetColor()
        {
            Type type = _data.GetType();
            return GetColor(type);
        }
    }
}
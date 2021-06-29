//=================================================
//
//    Created by jianzhou.yao
//
//=================================================

using System;
using System.Linq;
using UnityEngine.Playables;

namespace YaoJZ.Playable.Node
{
    public class PlayableOutputNodeView:UnityEditor.Experimental.GraphView.Node
    {
        private PlayableOutput _data;
        
        public PlayableOutputNodeView()
        {
         
        }

        public void SetData(PlayableOutput data)
        {
            _data = data;
            title = GetContentTypeShortName();
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
    }
}
using UnityEngine;
using UnityEngine.Playables;

namespace YaoJZ.Test.TimelineExtensions
{
    public class LoopClip:PlayableAsset
    {
        public LoopBehaviour Template = new LoopBehaviour();
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<LoopBehaviour>.Create(graph, Template);
        }
    }
}
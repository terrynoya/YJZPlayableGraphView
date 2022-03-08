using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

namespace YaoJZ.Playable.Node
{
    public class AnimationPlayableNodeView:PlayableNodeView
    {
        private ObjectField _animClipField;
        private Label _lblClipLength;
        public AnimationPlayableNodeView(UnityEngine.Playables.Playable playable) : base(playable)
        {
            
        }

        protected override void CreateChildren()
        {
            if (Data.GetPlayableType() != typeof(AnimationClipPlayable))
            {
                return;
            }

            AnimationClipPlayable animPlayable = (AnimationClipPlayable)Data;
            AnimationClip clip = animPlayable.GetAnimationClip();
            
            if(clip == null)
            {
                return;
            }

            _animClipField = new ObjectField();
            _animClipField.objectType = typeof(AnimationClip);
            _animClipField.value = clip;
            mainContainer.Add(_animClipField);
            
            _lblClipLength = new Label();
            mainContainer.Add(_lblClipLength);
            _lblClipLength.text = $"ClipLength:{clip.length}";
        }
    }
}
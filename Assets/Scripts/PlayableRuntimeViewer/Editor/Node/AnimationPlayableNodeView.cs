using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

namespace YaoJZ.Playable.Node
{
    public class AnimationPlayableNodeView:PlayableNodeViewBase
    {
        private ObjectField _animClipField;
        private Label _lblClipLength;
        public AnimationPlayableNodeView(UnityEngine.Playables.Playable playable) : base(playable)
        {
            
        }

        protected override void CreateChildren()
        {
            if (_playable.GetPlayableType() != typeof(AnimationClipPlayable))
            {
                return;
            }

            AnimationClipPlayable animPlayable = (AnimationClipPlayable)_playable;
            AnimationClip clip = animPlayable.GetAnimationClip();
            
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
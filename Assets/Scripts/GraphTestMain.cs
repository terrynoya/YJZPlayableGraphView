using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace YaoJZ.Test
{
    public class GraphTestMain:MonoBehaviour
    {
        public AnimationClip clip0;
        public AnimationClip clip1;

        private void Awake()
        {
            PlayableGraph graph = PlayableGraph.Create("test_graph");
            AnimationClipPlayable ap0 = AnimationClipPlayable.Create(graph, clip0);
            AnimationClipPlayable ap1 = AnimationClipPlayable.Create(graph, clip1);
            AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(graph, 2);
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            //graph.Connect(ap0, 0, mixer, 0);
            //graph.Connect(ap1, 0, mixer, 1);
            graph.Play();
        }
    }
}

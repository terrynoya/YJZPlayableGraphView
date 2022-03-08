using System;
using System.Linq;
using UnityEngine;

namespace YaoJZ.Playable.Node
{
    public abstract class GraphNodeView<T>:GraphNodeViewBase
    {
        protected T _data;

        public T Data => _data;

        public GraphNodeView(T data)
        {
            _data = data;
        }

        public static Color GetColor(Type type)
        {
            if (type == null)
                return Color.red;

            string shortName = type.ToString().Split('.').Last();
            float h = (float)Math.Abs(shortName.GetHashCode()) / int.MaxValue;
            return Color.HSVToRGB(h, 0.6f, 1.0f);
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
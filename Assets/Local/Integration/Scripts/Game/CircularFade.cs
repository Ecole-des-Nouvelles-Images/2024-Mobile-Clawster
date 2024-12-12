using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

namespace Local.Integration.Scripts.Game
{
    public class CircularFade : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material material = new Material(base.materialForRendering);
                material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return material;
            }
        }
        
    }
}

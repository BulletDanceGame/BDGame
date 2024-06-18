using UnityEngine;
using System.Collections.Generic;


namespace BulletDance.Maths
{
    //Taken from: https://github.com/NotWoods/unity-polygon-2d-editor

    public class BoundBox 
    {
        // -- Bounds defined by rendered objects -- //
        public static Bounds GetRenderedBounds(Transform parent)
        {
            List<Renderer> rendererList = new List<Renderer>();
            GetRenderedChildren(parent, rendererList);

            Vector3 center = Vector3.zero; 
            foreach(Renderer renderer in rendererList)
            {
                center += renderer.bounds.center;
            }
            center /= rendererList.Count; //center is average center of visible children

            Bounds bounds = new Bounds(center, Vector3.zero); 
            foreach(Renderer renderer in rendererList)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        private static void GetRenderedChildren(Transform parent, List<Renderer> rendererList)
        {
            var renderer = parent.GetComponent<Renderer>();
            if(renderer != null) rendererList.Add(renderer);

            foreach (Transform child in parent) 
            {
                GetRenderedChildren(child, rendererList);   
            }
        }
    }
}    

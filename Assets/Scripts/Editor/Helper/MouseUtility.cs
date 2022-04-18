using UnityEngine;
using UnityEditor;
using PathCreation;

namespace PathCreationEditor
{
    public static class MouseUtility
    {
        private const int raycastDepth = 99999;
        /// <summary>
		/// Determines mouse position in world. If PathSpace is xy/xz, the position will be locked to that plane.
		/// If PathSpace is xyz, then depthFor3DSpace will be used as distance from scene camera.
		/// </summary>
        public static Vector3 GetMouseWorldPosition(PathSpace space, float depthFor3DSpace = 10)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            Vector3 worldMouse;
            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, raycastDepth, LayerMask.GetMask("Ground")))
            {
                worldMouse = hit.point;
                //worldMouse = Camera.current.transform.InverseTransformPoint(hit.point);
            }
            else worldMouse = new Vector3(0,0,0);
            
            //Debug.DrawRay(mouseRay.origin, -mouseRay.direction*10f, Color.red);
            //Debug.Log("World Mouse: " + worldMouse);

            // Mouse can only move on XY plane
            if (space == PathSpace.xy)
            {
                float zDir = mouseRay.direction.z;
                if (zDir != 0)
                {
                    float dstToXYPlane = Mathf.Abs(mouseRay.origin.z / zDir);
                    worldMouse = mouseRay.GetPoint(dstToXYPlane);
                }
            } 

            // Mouse can only move on XZ plane 
            else if (space == PathSpace.xz)
            {
                float yDir = mouseRay.direction.y;
                if (yDir != 0)
                {
                    float dstToXZPlane = Mathf.Abs(mouseRay.origin.y / yDir);
                    worldMouse = mouseRay.GetPoint(dstToXZPlane);
                }
            }

            return worldMouse;
        }

    }
}
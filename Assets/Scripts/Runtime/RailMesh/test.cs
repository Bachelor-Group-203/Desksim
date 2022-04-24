using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class test : PathCreator
{

#if UNITY_EDITOR

    // Draw the path when path objected is not selected (if enabled in settings)
    void OnDrawGizmos()
    {

        // Only draw path gizmo if the path object is not selected
        // (editor script is resposible for drawing when selected)
        GameObject selectedObj = UnityEditor.Selection.activeGameObject;
        List<Vector3> nextPosition = new List<Vector3>();
        List<Vector3> position = new List<Vector3>();

        for (int i = 0; i < data.NumPoints; i++)
        {
            int nextI = i + 1;
            if (nextI >= data.NumPoints)
            {
                if (data.isClosedLoop)
                {
                    nextI %= data.NumPoints;
                }
                else
                {
                    break;
                }
            }
            Gizmos.DrawLine(data.GetPoint(i), data.localNormals[i] + data.GetPoint(nextI));
        }
    }
#endif
}

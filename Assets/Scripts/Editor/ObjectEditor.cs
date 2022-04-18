using System;
using System.Collections.Generic;
using PathCreation.Utility;
using PathCreation;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor; 
using UnityEditor.SceneManagement;

namespace PathCreationEditor
{
    [CustomEditor(typeof(ObjectOnPath))]

    /**
     * ObjectEditor allows for objects with "ObjectOnPath" scripts to easily place these objects on set path
     */
    public class ObjectEditor : Editor
    {
        EndOfPathInstruction end;
        ObjectOnPath objectOnPath;
        PathCreationEditor.ScreenSpacePolyLine screenSpaceLine;
        PathCreationEditor.ScreenSpacePolyLine.MouseInfo pathMouseInfo;
        Tool LastTool = Tool.None;
        Vector3 lastPoint;
        bool hasUpdatedScreenSpaceLine;
        float distanceTravelled;
        const float screenPolylineMaxAngleError = .3f;
        const float screenPolylineMinVertexDst = .01f;
        const float mouseDstToPathClamp = 30f;
    
        /**
         * When this script is enabled
         */
        private void OnEnable()
        {
            LastTool = Tools.current;
            Tools.current = Tool.None;
            GetLastPoint();
        }

        /**
         * When this script is disabled
         */
        private void OnDisable()
        {
            if (Application.isPlaying)
                return;

            Tools.current = LastTool;
        }
    
        /**
         * When on scene window
         */
        private void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();

            if (Application.isPlaying)
                return;

            // Get selected object that contains "ObjectOnPath.cs" script
            objectOnPath = (ObjectOnPath)target;

            if (!Application.isEditor || objectOnPath == null)
                return;
        
            // Cheks if the item has a pathcreator object if not finds the path object and assigns it
            if (pathCreator == null)
            {
                try
                {
                    objectOnPath.follower.pathCreator = GameObject.FindGameObjectWithTag("Rail").GetComponent<PathCreator>();
                }
                catch (Exception e)
                {
                    Debug.LogWarning("No path in the Scene: " + e);
                    return;
                }
            } 

            objectMouseHover();
        
            if (objectOnPath.transform.position == pathCreator.transform.position)
                return;

            // Set offset of objectOnPath to paths' offset for correct position
            objectOnPath.transform.position = pathCreator.transform.position;

            if (EditorGUI.EndChangeCheck())
            {
                // This code will unsave the current scene if there's any change in the editor GUI.
                // Hence user would forcefully need to save the scene before changing scene
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }
        }

        /**
         * ObjectMouseHover defines what occurs whilst hovering mouse on path
         */
        private void objectMouseHover() {
        
            //Get relative mouse position inside editor window
            UpdatePathMouseInfo ();

            Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;

            // If mouse is too far from path, then return train to last position
            if (pathMouseInfo.mouseDstToLine > mouseDstToPathClamp)
            {
                GetLastPoint();
                UpdateObject(lastPoint);
                return;
            }

            // Updates position of object used in Follower script
            UpdateObject(newPathPoint);

            // When clicking on path, place train and save last position
            if (Event.current.type == EventType.MouseDown && distanceTravelled > 0f) 
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                objectOnPath.follower.distanceOffset = distanceTravelled;
                objectOnPath.follower.distanceOffset = EditorGUILayout.FloatField("distanceOffset", distanceTravelled);
                SetLastPoint(newPathPoint);
            }
        }

        /**
         * Updates mouse info relative to the path
         */
        private void UpdatePathMouseInfo() {
            if (!hasUpdatedScreenSpaceLine || (screenSpaceLine != null && screenSpaceLine.TransformIsOutOfDate ())) {
                screenSpaceLine = new PathCreationEditor.ScreenSpacePolyLine (bezierPath, objectOnPath.transform, screenPolylineMaxAngleError, screenPolylineMinVertexDst);
                hasUpdatedScreenSpaceLine = true;
            }
            if (screenSpaceLine != null)
                pathMouseInfo = screenSpaceLine.CalculateMouseInfo ();
        }

        /**
         * Updates train position and signal position
         *
         * @param       point       Vector3 point to place the train
         */
        private void UpdateObject (Vector3 point) {

            // Transform objects' position to mouse position
            point = MathUtility.InverseTransformPoint (point, objectOnPath.transform, bezierPath.Space);
            point += pathCreator.transform.position;

            // Get distance between mouse and path
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(point);
            if (distanceTravelled > 0f)
            {
                Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
                Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
                model.transform.position = point;
                model.transform.rotation = pathRotation * normalRotation;

                //if object is signal instead of train
                if (objectOnPath.follower.isSignal)
                {
                    objectOnPath.objectOffset = pathCreator.path.GetNormalAtDistance(distanceTravelled);

                    model.transform.position += objectOnPath.objectOffset.normalized * objectOnPath.offsetDistance;
                    model.transform.rotation *= Quaternion.Euler(0, 180, 0);
                    model.GetComponentInParent<SignalScript>().MoveBoxColliders(new Vector3(-objectOnPath.offsetDistance, 0, -20));
                }
            }
        }

        /**
         * Gets last point inside EditorPrefs
         */
        private void GetLastPoint() {
            float xstring = EditorPrefs.GetFloat("xLastPoint");
            float ystring = EditorPrefs.GetFloat("yLastPoint");
            float zstring = EditorPrefs.GetFloat("zLastPoint");
            lastPoint = new Vector3(xstring, ystring, zstring);
        }

        /**
         * Sets last point inside EditorPrefs
         */
        private void SetLastPoint(Vector3 vec) {
            EditorPrefs.SetFloat("xLastPoint", vec.x);
            EditorPrefs.SetFloat("yLastPoint", vec.y);
            EditorPrefs.SetFloat("zLastPoint", vec.z);
            GetLastPoint();
        }

        BezierPath bezierPath 
        {
            get {
                return objectOnPath.follower.pathCreator.EditorData.bezierPath;
            }
        }
        PathCreator pathCreator
        {
            get {
                return objectOnPath.follower.pathCreator;
            }
        }
        GameObject model
        {
            get {
                return objectOnPath.follower.model;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using PathCreation.Utility;
using PathCreation;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor; 
using UnityEditor.SceneManagement;

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
        if (Application.isPlaying)
            return;

        objectOnPath = (ObjectOnPath)target;

        if (!Application.isEditor || objectOnPath == null)
            return;

        objectMouseHover();
        
        if (objectOnPath.transform.position == pathCreator.transform.position)
            return;

        objectOnPath.transform.position = pathCreator.transform.position;
    }

    /**
     * ObjectMouseHover defines what occurs whilst hovering mouse on path
     */
    private void objectMouseHover() {
        
        UpdatePathMouseInfo ();
        
        SetObjectOffset(objectOnPath.objectOffset);

        Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;

        // If mouse is too far from path, then return train to last position
        if (pathMouseInfo.mouseDstToLine > mouseDstToPathClamp)
        {
            GetLastPoint();
            UpdateTrain(lastPoint);
            return;
        }

        UpdateTrain(newPathPoint);

        // When clicking on path, place train and save last position
        if (Event.current.type == EventType.MouseDown && distanceTravelled > 0f) 
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            objectOnPath.follower.UpdateDistanceOffset(distanceTravelled);
            EditorPrefs.SetFloat((string)objectOnPath.gameObject.name, distanceTravelled);
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
     * Updates train position
     *
     * @param       point       Vector3 point to place the train
     */
    private void UpdateTrain (Vector3 point) {
        point = MathUtility.InverseTransformPoint (point, objectOnPath.transform, bezierPath.Space);
        point += pathCreator.transform.position;
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(point);
        if (distanceTravelled > 0f)
        {
            Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
            Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
            model.transform.position = point;
            if (objectOnPath.objectOffset != new Vector3(0, 0, 0))
                model.transform.position += objectOnPath.objectOffset;
            model.transform.rotation = pathRotation * normalRotation;
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

    /**
     * Sets offset of object on path (used for signals)
     */
    private void SetObjectOffset(Vector3 vec) {
        EditorPrefs.SetFloat("x" + (string)objectOnPath.gameObject.name, vec.x);
        EditorPrefs.SetFloat("y" + (string)objectOnPath.gameObject.name, vec.y);
        EditorPrefs.SetFloat("z" + (string)objectOnPath.gameObject.name, vec.z);
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
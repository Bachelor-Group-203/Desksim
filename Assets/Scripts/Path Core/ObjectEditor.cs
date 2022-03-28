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
    ObjectOnPath trainOnPath;
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

        trainOnPath = (ObjectOnPath)target;

        if (!Application.isEditor || trainOnPath == null)
            return;

        objectMouseHover();
        
        if (trainOnPath.transform.position == pathCreator.transform.position)
            return;

        trainOnPath.transform.position = pathCreator.transform.position;
    }

    /**
     * ObjectMouseHover defines what occurs whilst hovering mouse on path
     */
    private void objectMouseHover() {
        
        UpdatePathMouseInfo ();

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
            trainOnPath.follower.UpdateDstOffset(distanceTravelled);
            EditorPrefs.SetFloat("dstOffset", distanceTravelled);
            SetLastPoint(newPathPoint);
        }
    }

    /**
     * Updates mouse info relative to the path
     */
    private void UpdatePathMouseInfo() {
        if (!hasUpdatedScreenSpaceLine || (screenSpaceLine != null && screenSpaceLine.TransformIsOutOfDate ())) {
            screenSpaceLine = new PathCreationEditor.ScreenSpacePolyLine (bezierPath, trainOnPath.transform, screenPolylineMaxAngleError, screenPolylineMinVertexDst);
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
        point = MathUtility.InverseTransformPoint (point, trainOnPath.transform, bezierPath.Space);
        point += pathCreator.transform.position;
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(point);
        if (distanceTravelled > 0f)
        {
            Quaternion normalRotation = Quaternion.Euler(180, 0, 90); 
            Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
            train.transform.position = point;
            train.transform.rotation = pathRotation * normalRotation;
        }
    }

    /**
     * Gets last point inside EditorPrefs
     */
    private void GetLastPoint() {
        float xstring = EditorPrefs.GetFloat("xfloat");
        float ystring = EditorPrefs.GetFloat("yfloat");
        float zstring = EditorPrefs.GetFloat("zfloat");
        lastPoint = new Vector3(xstring, ystring, zstring);
    }

    /**
     * Sets last point inside EditorPrefs
     */
    private void SetLastPoint(Vector3 vec) {
        EditorPrefs.SetFloat("xfloat", vec.x);
        EditorPrefs.SetFloat("yfloat", vec.y);
        EditorPrefs.SetFloat("zfloat", vec.z);
        GetLastPoint();
    }

    BezierPath bezierPath 
    {
        get {
            return trainOnPath.follower.pathCreator.EditorData.bezierPath;
        }
    }
    PathCreator pathCreator
    {
        get {
            return trainOnPath.follower.pathCreator;
        }
    }
    GameObject train
    {
        get {
            return trainOnPath.follower.train;
        }
    }
}
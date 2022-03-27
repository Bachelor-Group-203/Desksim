using System.Collections;
using System.Collections.Generic;
using PathCreation.Utility;
using PathCreation;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor; 
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TrainOnPath))]
public class TrainEditor : Editor
{
    EndOfPathInstruction end;
    TrainOnPath trainOnPath;
    PathCreationEditor.ScreenSpacePolyLine screenSpaceLine;
    PathCreationEditor.ScreenSpacePolyLine.MouseInfo pathMouseInfo;
    Tool LastTool = Tool.None;
    Vector3 lastPoint;
    bool hasUpdatedScreenSpaceLine;
    float distanceTravelled;
    const float screenPolylineMaxAngleError = .3f;
    const float screenPolylineMinVertexDst = .01f;
    const float mouseDstToPathClamp = 30f;
    
    private void OnSceneGUI()
    {
        if (Application.isPlaying)
            return;

        trainOnPath = (TrainOnPath)target;

        if (!Application.isEditor && trainOnPath.follower.frontAttachment)
            return;

        objectMouseHover(Event.current);
        
        if (trainOnPath.transform.position == pathCreator.transform.position)
            return;

        trainOnPath.transform.position = pathCreator.transform.position;
    }
    private void OnEnable()
    {
        LastTool = Tools.current;
        Tools.current = Tool.None;
        GetLastPoint();
    }
    private void OnDisable()
    {
        if (Application.isPlaying)
            return;

        Tools.current = LastTool;
    }
    private void objectMouseHover(Event e) {
        if (trainOnPath == null)
            return;
        
        UpdatePathMouseInfo ();

        Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;

        if (e.type == EventType.MouseDown && distanceTravelled > 0f) 
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            trainOnPath.follower.UpdateDstOffset(distanceTravelled);
            EditorPrefs.SetFloat("dstOffset", distanceTravelled);
            SetLastPoint(newPathPoint);
        }
        if (pathMouseInfo.mouseDstToLine <= mouseDstToPathClamp)
        {
            UpdateTrain(newPathPoint);
        }
        else 
        {
            GetLastPoint();
            UpdateTrain(lastPoint);
        }
    }

    private void UpdatePathMouseInfo() {
        if (!hasUpdatedScreenSpaceLine || (screenSpaceLine != null && screenSpaceLine.TransformIsOutOfDate ())) {
            screenSpaceLine = new PathCreationEditor.ScreenSpacePolyLine (bezierPath, trainOnPath.transform, screenPolylineMaxAngleError, screenPolylineMinVertexDst);
            hasUpdatedScreenSpaceLine = true;
        }
        pathMouseInfo = screenSpaceLine.CalculateMouseInfo ();
    }
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
    private void GetLastPoint() {
        float xstring = EditorPrefs.GetFloat("xfloat");
        float ystring = EditorPrefs.GetFloat("yfloat");
        float zstring = EditorPrefs.GetFloat("zfloat");
        lastPoint = new Vector3(xstring, ystring, zstring);
    }
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
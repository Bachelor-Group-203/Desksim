using System.Collections;
using System.Collections.Generic;
using PathCreation.Utility;
using PathCreation;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor; 

[CustomEditor(typeof(TrainOnPath))]
public class TrainEditor : Editor
{
    public EndOfPathInstruction end;
    private TrainOnPath trainOnPath;
    PathCreationEditor.ScreenSpacePolyLine screenSpaceLine;
    PathCreationEditor.ScreenSpacePolyLine.MouseInfo pathMouseInfo;
    Tool LastTool = Tool.None;
    bool hasUpdatedScreenSpaceLine;
    private float distanceTravelled;
    private float newDstOffset;
    const float screenPolylineMaxAngleError = .3f;
    const float screenPolylineMinVertexDst = .01f;
    const float mouseDstToPathClamp = 30f;

    //TODO Load correct train position after sceneload
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
    }
    private void OnDisable()
    {
        if (Application.isPlaying)
            return;

        Tools.current = LastTool;
        trainOnPath.newDstOffset = newDstOffset;
    }
    private void objectMouseHover(Event e) {
        if (trainOnPath == null)
            return;
        
        UpdatePathMouseInfo ();

        Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;

        if (e.type == EventType.MouseDown && distanceTravelled > 0f) 
        {
            newDstOffset = distanceTravelled;
        }
        if (pathMouseInfo.mouseDstToLine <= mouseDstToPathClamp)
        {
            newPathPoint = MathUtility.InverseTransformPoint (newPathPoint, trainOnPath.transform, bezierPath.Space);
            newPathPoint += pathCreator.transform.position;
            //Debug.Log("Pos: " + newPathPoint);
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(newPathPoint);
            if (distanceTravelled > 0f)
            {
                Quaternion normalRotation = Quaternion.Euler(180, 0, 90); 
                Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
                train.transform.position = newPathPoint;
                train.transform.rotation = pathRotation * normalRotation;
            }
        }
    }

    void UpdatePathMouseInfo () {
        if (!hasUpdatedScreenSpaceLine || (screenSpaceLine != null && screenSpaceLine.TransformIsOutOfDate ())) {
            screenSpaceLine = new PathCreationEditor.ScreenSpacePolyLine (bezierPath, trainOnPath.transform, screenPolylineMaxAngleError, screenPolylineMinVertexDst);
            hasUpdatedScreenSpaceLine = true;
        }
        pathMouseInfo = screenSpaceLine.CalculateMouseInfo ();
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
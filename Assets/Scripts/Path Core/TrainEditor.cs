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
    const float screenPolylineMaxAngleError = .3f;
    const float screenPolylineMinVertexDst = .01f;
    const float mouseDstToPathClamp = 30f;
    //private Vector3 closestPosition;

    private void OnSceneGUI()
    {
        trainOnPath = (TrainOnPath)target; 
        objectMouseHover();
    }
    private void objectMouseHover() {
        //TODO Add placement functionality and fix weird error
        if (trainOnPath != null)
        {
            UpdatePathMouseInfo ();

            Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;

            if (pathMouseInfo.mouseDstToLine <= mouseDstToPathClamp)
            {
                newPathPoint = MathUtility.InverseTransformPoint (newPathPoint, trainOnPath.transform, bezierPath.Space);
                newPathPoint += pathCreator.transform.position;
                train.transform.position = newPathPoint;
                float distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(newPathPoint);
                Quaternion normalRotation = Quaternion.Euler(180, 0, 90); 
                Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
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
 
    void OnEnable()
    {
        LastTool = Tools.current;
        Tools.current = Tool.None;
    }
 
    void OnDisable()
    {
        Tools.current = LastTool;
    }
    BezierPath bezierPath 
    {
        get {
            return trainOnPath.pathCreator.EditorData.bezierPath;
        }
    }
    PathCreator pathCreator
    {
        get {
            return trainOnPath.pathCreator;
        }
    }
    GameObject train
    {
        get {
            return trainOnPath.train;
        }
    }
}
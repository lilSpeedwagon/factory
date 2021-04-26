using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public interface IMover
{
    IMover Next { get; }
    float Height { get; }
    MotionScript ReleaseMotion();
    void Move(MotionScript motionObject);
    bool IsAbleToMove();
    bool IsDirectionAllowed(TileUtils.Direction direction);
    bool IsFree();
    void HoldMotion(MotionScript obj);
}

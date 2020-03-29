using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public interface IMover
{
    IMover Next { get; }
    void Move(MotionScript motionObject);
    bool IsAbleToMove();
    bool IsDirectionAllowed(TileUtils.Direction direction);
    bool IsFree();
    void HoldMotion(MotionScript obj);
}

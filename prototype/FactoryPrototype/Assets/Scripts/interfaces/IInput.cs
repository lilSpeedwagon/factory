using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInput
{
    bool IsReadyToConsume { get; }
    void Consume(MotionScript obj);
}

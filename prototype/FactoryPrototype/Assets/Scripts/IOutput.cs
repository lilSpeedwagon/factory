using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOutput : IMover
{
    bool IsReadyToEmit();
    void Emit();
}

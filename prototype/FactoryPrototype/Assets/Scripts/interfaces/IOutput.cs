using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOutput : IMover
{
    bool IsReadyToEmit { get; }
    void Emit();
    string MaterialToEmit { get; set; }
}

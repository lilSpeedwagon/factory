using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProcessor
{
    bool CanProcess(MaterialType type);
    GameObject Process(IProcessable obj);
}

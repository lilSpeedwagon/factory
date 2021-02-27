using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProcessor
{
    bool CanProcess(string material);
    Material Process(Material obj);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProcessor
{
    public string Name { get; }
    bool CanProcess(string material);
    Material Process(Material obj);
}

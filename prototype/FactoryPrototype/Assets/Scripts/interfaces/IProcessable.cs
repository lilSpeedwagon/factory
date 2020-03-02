using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProcessable
{
    MaterialType Type { get; set; }
    GameObject gameObject { get; }
}

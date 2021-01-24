using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class DataValue
{
    public enum DataType
    {
        Undefined,
        Number,
        Bool,
        String
    }

    public DataType Type;
}

class DataValueNumber : DataValue
{
    public DataValueNumber(float value)
    {
        Type = DataType.Number;
        Value = value;
    }
    public float Value;
}

class DataValueBool : DataValue
{
    public DataValueBool(bool value)
    {
        Type = DataType.Bool;
        Value = value;
    }
    public bool Value;
}

class DataValueString : DataValue
{
    public DataValueString(string value)
    {
        Type = DataType.String;
        Value = value;
    }
    public string Value;
}

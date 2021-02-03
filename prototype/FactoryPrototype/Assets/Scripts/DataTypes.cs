using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class DataValue
{
    public enum DataType
    {
        Undefined,
        Number,
        Bool,
        String
    }

    public DataType Type;

    public DataValue(float value)
    {
        m_value = value;
        Type = DataType.Number;
    }

    public DataValue(bool value)
    {
        m_value = value;
        Type = DataType.Bool;
    }

    public DataValue(string value)
    {
        m_value = value;
        Type = DataType.String;
    }

    public string GetString()
    {
        return Type == DataType.Undefined ? "" : m_value.ToString();
    }

    public float GetNumber()
    {
        if (Type == DataType.String)
        {
            string strValue = (string) m_value;
            try
            {
                return float.Parse(strValue);
            }
            catch (FormatException) {}

            return 0.0f;
        }

        if (Type == DataType.Bool)
        {
            bool boolValue = (bool) m_value;
            return boolValue ? 1.0f : 0.0f;
        }

        return (float) m_value;
    }

    public bool GetBool()
    {
        switch (Type)
        {
            case DataType.String:
            {
                string strValue = (string) m_value;
                return strValue.ToLower() == "true";
            }
            case DataType.Number:
            {
                float value = (float) m_value;
                return value > 0;
            }
            case DataType.Bool:
                return (bool)m_value;
            default:
                throw new Exception("Trying to get value from undefined object.");
        }
    }

    private DataValue()
    {
        Type = DataType.Undefined;
    }

    private readonly object m_value;
}

/*class DataValueNumber : DataValue
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
}*/


using System;
using UnityEngine;

/// <summary>
/// Generic array for classes subclasses to use different array types
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable] 
public class GenericArray<T> 
{
    private int _arraySize;
    private T[] _array;
    public GenericArray(int size)
    {
        _array = new T[size];
    }

    public T GetIndex(int index)
    {
        return _array[index];
    }

    public int GetSize()
    {
        return _arraySize;
    }
} 


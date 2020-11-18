using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper to find a child of a transform. Thanks to the Unity community for this implementation.
/// https://answers.unity.com/questions/799429/transformfindstring-no-longer-finds-grandchild.html
/// </summary>
public static class TransformRecursiveChildExtension
{
    //Breadth-first search
    public static Transform FindChildRecursive(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindChildRecursive(aName);
            if (result != null)
                return result;
        }
        return null;
    }
}

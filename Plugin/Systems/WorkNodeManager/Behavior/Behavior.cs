using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace NodeEditor
{
    public abstract class Behavior<T> : BaseAction<T> where T : ObjectManager
    {
    }
}
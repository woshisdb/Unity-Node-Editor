using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdAttribute : Attribute
{
    public IdAttribute(string name)
    {
        this.name = name;
    }

    public string name { get; set; }
}

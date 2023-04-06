using System;
using System.Collections;
using System.Collections.Generic;
using NodeEditor;
using UnityEngine;

public class IdAttribute : Attribute
{
    public IdAttribute(string name)
    {
        this.name = name;
    }
    public IdAttribute(string name,bool flag)
    {
        this.name = name;
        if(flag)
        this.privateEnum = PrivateEnum.Public;
    }
    public IdAttribute(string name, bool flag,bool isvirture)
    {
        this.name = name;
        if (flag)
            this.privateEnum = PrivateEnum.Public;
        this.isVirtre = isvirture;
    }
    public string name { get; set; }
    public PrivateEnum privateEnum {get;set;}
    public bool isVirtre { get; set; }
}

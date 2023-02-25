using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NodeEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditor
{
    public class EnumElemnet : BaseElement
    {
        EnumField field;
        public EnumElemnet(Type type, string Name, BaseNode baseNode) : base(type, Name, baseNode)
        {
            Enum temp = Activator.CreateInstance(type) as Enum;
            EnumField objectField = new EnumField();
            objectField.label = Name;
            objectField.Init(temp);
            name = Name;
            Add(objectField);
            field = objectField;
        }

        public override object GetVal()
        {
            return field.value;
        }

        public override void SetVal(object val)
        {
            field.value = (Enum)val;
        }
    }
    public class StringElemnet : BaseElement
    {
        TextField field;
        public StringElemnet(Type type, string Name, BaseNode baseNode) : base(type, Name, baseNode)
        {
            TextField objectField = new TextField();
            objectField.multiline = true;
            objectField.style.flexWrap = Wrap.Wrap;
            name = Name;
            objectField.label = Name;
            objectField.style.width = 300f;
            objectField.style.whiteSpace = WhiteSpace.Normal;
            Add(objectField);
            field = objectField;
        }

        public override object GetVal()
        {
            return field.value;
        }

        public override void SetVal(object val)
        {
            field.value = (string)val;
        }
    }

    public class IntegerElemnet : BaseElement
    {
        IntegerField field;
        public IntegerElemnet(Type type, string Name, BaseNode baseNode) : base(type, Name, baseNode)
        {
            IntegerField objectField = new IntegerField();
            name = Name; ;
            objectField.label = Name;
            Add(objectField);
            field = objectField;
        }

        public override object GetVal()
        {
            return field.value;
        }

        public override void SetVal(object val)
        {
            field.value = (int)val;
        }
    }

    public class FloatElemnet : BaseElement
    {
        FloatField field;
        public FloatElemnet(Type type, string Name, BaseNode baseNode) : base(type, Name, baseNode)
        {
            FloatField objectField = new FloatField();
            name = Name;
            objectField.label = Name;
            Add(objectField);
            field = objectField;
        }

        public override object GetVal()
        {
            return field.value;
        }

        public override void SetVal(object val)
        {
            //Debug.Log(val);
            float temp = (float)val;
            field.value = temp;
        }
    }

    public class BoolElemnet : BaseElement
    {
        Toggle field;
        public BoolElemnet(Type type, string Name, BaseNode baseNode) : base(type, Name, baseNode)
        {
            Toggle objectField = new Toggle();
            name = Name;
            objectField.label = Name;
            Add(objectField);
            field = objectField;
        }

        public override object GetVal()
        {
            return field.value;
        }

        public override void SetVal(object val)
        {
            field.value = (bool)val;
        }
    }

    public class ObjectElemnet : BaseElement
    {
        ObjectField field;
        public ObjectElemnet(Type type, string Name, BaseNode baseNode) : base(type, Name, baseNode)
        {
            ObjectField objectField = new ObjectField();
            objectField.objectType = type;
            name = Name;
            objectField.label = Name;
            Add(objectField);
            field = objectField;
        }

        public override object GetVal()
        {
            return field.value;
        }

        public override void SetVal(object val)
        {
            field.value = (UnityEngine.Object)val;
        }
    }
}
﻿// Decompiled with JetBrains decompiler
// Assembly: UnityEngine.UIElementsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Unity 2022.2.4f1

using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Editor
{

//Taken from here: https://github.com/achimmihca/Unity-UIToolkit-ListViewHorizontal


public interface IDragAndDropData
{
    object GetGenericData(string key);

    object userData { get; }

    IEnumerable<Object> unityObjectReferences { get; }
}

}
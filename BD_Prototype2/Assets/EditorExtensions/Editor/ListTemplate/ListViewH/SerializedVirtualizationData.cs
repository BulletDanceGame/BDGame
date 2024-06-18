// Decompiled with JetBrains decompiler
// Assembly: UnityEngine.UIElementsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Unity 2022.2.4f1

using System;
using UnityEngine;

namespace BulletDance.Editor
{

//Taken from here: https://github.com/achimmihca/Unity-UIToolkit-ListViewHorizontal


[Serializable]
public class SerializedVirtualizationData
{
    public Vector2 scrollOffset;
    public int firstVisibleIndex;
    public float contentPadding;
    public float contentWidth;
    public int anchoredItemIndex;
    public float anchorOffset;
}

}
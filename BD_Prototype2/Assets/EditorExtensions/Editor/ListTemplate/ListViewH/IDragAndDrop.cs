﻿// Decompiled with JetBrains decompiler
// Assembly: UnityEngine.UIElementsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Unity 2022.2.4f1
namespace BulletDance.Editor
{

//Taken from here: https://github.com/achimmihca/Unity-UIToolkit-ListViewHorizontal


internal interface IDragAndDrop
{
    void StartDrag(StartDragArgs args);

    void AcceptDrag();

    void SetVisualMode(DragVisualMode visualMode);

    IDragAndDropData data { get; }
}

}
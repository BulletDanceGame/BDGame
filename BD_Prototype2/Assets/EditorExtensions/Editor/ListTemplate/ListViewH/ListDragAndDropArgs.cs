// Decompiled with JetBrains decompiler
// Assembly: UnityEngine.UIElementsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Unity 2022.2.4f1

namespace BulletDance.Editor
{

//Taken from here: https://github.com/achimmihca/Unity-UIToolkit-ListViewHorizontal

internal struct ListDragAndDropArgs : IListDragAndDropArgs
{
    public object target { get; set; }

    public int insertAtIndex { get; set; }

    public DragAndDropPosition dragAndDropPosition { get; set; }

    public IDragAndDropData dragAndDropData { get; set; }
}
}
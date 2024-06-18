// Decompiled with JetBrains decompiler
// Assembly: UnityEngine.UIElementsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Unity 2022.2.4f1

namespace BulletDance.Editor
{

//Taken from here: https://github.com/achimmihca/Unity-UIToolkit-ListViewHorizontal


public interface IListDragAndDropArgs
{
    object target { get; }

    int insertAtIndex { get; }

    IDragAndDropData dragAndDropData { get; }

    DragAndDropPosition dragAndDropPosition { get; }
}

}
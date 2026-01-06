using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBookColor
{
    Red,
    Blue,
    Whatever
}

public enum EBookShape
{
    Triangle,
    Rectangle,
    Whatever
}

public enum EBookLineType
{
    Solid,
    Dashed,
    Whatever
}

public struct BookType
{
    public EBookColor color;
    public EBookShape shape;
    public EBookLineType lineType;
}

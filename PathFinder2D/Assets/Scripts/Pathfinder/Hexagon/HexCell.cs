using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 每个单元都有六个邻居，可以使用指南针方向表示它们
public enum HexDirection {
    NE, E, SE, SW, W, NW
}

// 扩展方法
// 拓展方法是一个静态类中的静态方法，表现类似于一些类型的实例方法。
// 可以是任何类型，一个类、一个借口、一个结构体、一个原始值、或者一个枚举。
// 拓展方法的第一个参数需要有this关键词。
// 它定义了方法将要操作的类型和实例的值。
public static class HexDirectionExtensions {
    public static HexDirection Opposite(this HexDirection direction) {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    // 跳到前一个方向
    public static HexDirection Previous(this HexDirection direction) {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    // 跳到下一个方向
    public static HexDirection Next(this HexDirection direction) {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
}

public class HexCell : BaseCell {

    #region 开放接口

    /// <summary>
    /// 得到某方向上一个单元的邻居。由于方向总是在0到5之间，我们不需要检查索引（index）是否在数组的界限范围内。
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public HexCell GetNeighbor(HexDirection direction) {
        if (block)
            return null;
        if (neighbors[(int)direction].block)
            return null;
        return neighbors[(int)direction] as HexCell;
    }

    /// <summary>
    /// 设置邻居
    /// 邻居关系是双向的。所以在一个方向建立起邻居时，我们可以立刻将邻居设为相对方向。
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="cell"></param>
    public void SetNeighbor(HexDirection direction, HexCell cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    #endregion

}

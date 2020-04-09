using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquareDirection {
    E, S, W, N
}

// 扩展方法
// 拓展方法是一个静态类中的静态方法，表现类似于一些类型的实例方法。
// 可以是任何类型，一个类、一个借口、一个结构体、一个原始值、或者一个枚举。
// 拓展方法的第一个参数需要有this关键词。
// 它定义了方法将要操作的类型和实例的值。
public static class SquareDirectionExtensions {
    public static SquareDirection Opposite(this SquareDirection direction) {
        return (int)direction < 2 ? (direction + 2) : (direction - 2);
    }

    // 跳到前一个方向
    public static SquareDirection Previous(this SquareDirection direction) {
        return direction == SquareDirection.E ? SquareDirection.N : (direction - 1);
    }

    // 跳到下一个方向
    public static SquareDirection Next(this SquareDirection direction) {
        return direction == SquareDirection.N ? SquareDirection.E : (direction + 1);
    }
}

public class SquareCell : BaseCell {

    #region 开放接口

    /// <summary>
    /// 得到某方向上一个单元的邻居。由于方向总是在0到3之间，我们不需要检查索引（index）是否在数组的界限范围内。
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public SquareCell GetNeighbor(SquareDirection direction) {
        if (block)
            return null;
        if (neighbors[(int)direction].block)
            return null;
        return neighbors[(int)direction] as SquareCell;
    }

    /// <summary>
    /// 设置邻居
    /// 邻居关系是双向的。所以在一个方向建立起邻居时，我们可以立刻将邻居设为相对方向。
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="cell"></param>
    public virtual void SetNeighbor(SquareDirection direction, SquareCell cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    #endregion

}

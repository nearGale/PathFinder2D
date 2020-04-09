using System;
using System.Diagnostics;
using UnityEngine;

public class Node<T>
{
    public T Data { set; get; }          //数据域,当前结点数据
    public Node<T> Next { set; get; }    //位置域,下一个结点地址

    public Node(T item)
    {
        this.Data = item;
        this.Next = null;
    }

    public Node()
    {
        this.Data = default(T);
        this.Next = null;
    }
}

public class LinkList<T>
{
    public Node<T> Head { set; get; } //单链表头

    //构造
    public LinkList()
    {
        Clear();
    }

    /// <summary>
    /// 求单链表的长度
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
        Node<T> p = Head;
        int length = 0;
        while (p != null)
        {
            p = p.Next;
            length++;
        }
        return length;
    }

    /// <summary>
    /// 判断单键表是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        if (Head == null)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 清空单链表
    /// </summary>
    public void Clear()
    {
        Head = null;
    }

    /// <summary>
    /// 获得当前位置单链表中结点的值
    /// </summary>
    /// <param name="i">结点位置</param>
    /// <returns></returns>
    public T GetNodeValue(int i)
    {
        if (IsEmpty() || i < 0 || i >= GetLength())
        {
            UnityEngine.Debug.Log("单链表为空或结点位置有误！");
            return default(T);
        }

        Node<T> A = new Node<T>();
        A = Head;
        int j = 0;
        while (A.Next != null && j < i)
        {
            A = A.Next;
            j++;
        }

        return A.Data;
    }

    /// <summary>
    /// 增加新元素到单链表末尾
    /// </summary>
    public void Append(T item)
    {
        Node<T> foot = new Node<T>(item);
        Node<T> A = new Node<T>();
        if (Head == null)
        {
            Head = foot;
            return;
        }
        A = Head;
        while (A.Next != null)
        {
            A = A.Next;
        }
        A.Next = foot;
    }

    /// <summary>
    /// 增加单链表插入的位置
    /// </summary>
    /// <param name="item">结点内容</param>
    /// <param name="n">结点插入的位置</param>
    public void Insert(T item, int n)
    {
        if (IsEmpty() || n < 0 || n >= GetLength())
        {
            UnityEngine.Debug.Log("单链表为空或结点位置有误！");
            return;
        }

        if (n == 0)  //增加到头部
        {
            Node<T> H = new Node<T>(item);
            H.Next = Head;
            Head = H;
            return;
        }

        Node<T> A = new Node<T>();
        Node<T> B = new Node<T>();
        B = Head;
        int j = 0;
        while (B.Next != null && j < n)
        {
            A = B;
            B = B.Next;
            j++;
        }

        if (j == n)
        {
            Node<T> C = new Node<T>(item);
            A.Next = C;
            C.Next = B;
        }
    }

    /// <summary>
    /// 删除单链表结点
    /// </summary>
    /// <param name="i">删除结点位置</param>
    /// <returns></returns>
    public void RemoveAt(int i)
    {
        if (i < 0)
            i = GetLength() + i;

        if (IsEmpty() || i < 0 || i >= GetLength())
        {
            UnityEngine.Debug.Log("单链表为空或结点位置有误！");
            return;
        }

        Node<T> A = new Node<T>();
        if (i == 0)   //删除头
        {
            A = Head;
            Head = Head.Next;
            return;
        }
        Node<T> B = new Node<T>();
        B = Head;
        int j = 0;
        while (B.Next != null && j < i)
        {
            A = B;
            B = B.Next;
            j++;
        }
        if (j == i)
        {
            A.Next = B.Next;
        }
    }

    public void Remove(T item)
    {
        if (IsEmpty() )
        {
            UnityEngine.Debug.Log("单链表为空！");
            return;
        }

        Node<T> A = new Node<T>();
        if (item.Equals(Head.Data))   //删除头
        {
            A = Head;
            Head = Head.Next;
            return;
        }
        Node<T> B = new Node<T>();
        B = Head;
        while (B.Next != null)
        {
            A = B;
            B = B.Next;
            if (item.Equals(B.Data))
            {
                A.Next = B.Next;
            }
        }
    }

    /// <summary>
    /// 显示单链表
    /// </summary>
    public void Dispaly()
    {
        Node<T> A = new Node<T>();
        A = Head;
        while (A != null)
        {
            UnityEngine.Debug.Log(A.Data);
            A = A.Next;
        }
    }

    #region 面试题
    /// <summary>
    /// 单链表反转
    /// </summary>
    public void Reverse()
    {
        if (GetLength() == 1 || Head == null)
        {
            return;
        }

        Node<T> NewNode = null;
        Node<T> CurrentNode = Head;
        Node<T> TempNode = new Node<T>();

        while (CurrentNode != null)
        {
            TempNode = CurrentNode.Next;
            CurrentNode.Next = NewNode;
            NewNode = CurrentNode;
            CurrentNode = TempNode;
        }
        Head = NewNode;

        Dispaly();
    }

    /// <summary>
    /// 获得单链表中间值
    /// 思路：使用两个指针，第一个每次走一步，第二个每次走两步：
    /// </summary>
    public void GetMiddleValue()
    {
        Node<T> A = Head;
        Node<T> B = Head;

        while (B != null && B.Next != null)
        {
            A = A.Next;
            B = B.Next.Next;
        }
        if (B != null) //奇数
        {
            Console.WriteLine("奇数:中间值为：{0}", A.Data);
        }
        else    //偶数
        {
            Console.WriteLine("偶数:中间值为：{0}和{1}", A.Data, A.Next.Data);
        }
    }

    #endregion

}
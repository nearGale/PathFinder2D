using System;

public abstract class Singleton<T> where T : new()
{
    protected Singleton() { }
    public static T Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new T();
            }
            return instance;
        }
    }

    private static T instance;

    public virtual void Init()
    {
        OnInit();
    }

    protected virtual void OnInit() { }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static string ListToString<T>(List<T> list, string spliter = ",")
    {
        string str = "";

        int i = 0;
        foreach(var item in list)
        {
            if(i != 0)
            {
                str += spliter;
            }

            str += item.ToString();
            i++;
        }

        return str;
    }
}

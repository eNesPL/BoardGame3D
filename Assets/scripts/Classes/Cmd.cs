using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityToolbag;

public class Cmd 
{
    public string cmd;
    public bool hasReturn=false;
    public Action<JObject> JObfunc;
    public Action func;
    public int dice;
    public Cmd(string cmd)
    {
        this.cmd = cmd;
        this.hasReturn = false;
    }

    public Cmd(string cmd, bool hasReturn, Action func)
    {
        this.cmd = cmd;
        this.hasReturn = hasReturn;
        this.func = func;
    }
    public Cmd(string cmd, bool hasReturn, Action<JObject> Jfunc)
    {
        this.cmd = cmd;
        this.hasReturn = hasReturn;
        this.JObfunc = Jfunc;
    }

}

using System.Collections;
using UnityEngine;

public class Hello
{
    public static void Run()
    {
        Debug.Log("Hello, HybridCLR ! ========================================================================= ");

        /* 
           adb -s 127.0.0.1:16384 push HotUpdate.dll.bytes /sdcard/Android/data/com.Ksh.Hybrid001/files/Res/Assembly/
           替换成下面新语句后，点击菜单HybridCLR/CompileDll/ActiveBuildTarget，然后重命名成HotUpdate.dll.bytes后，拷贝到手机
        */
        //Debug.Log("Hello, HybridCLR ! ========================================================================= HotUpdated 1617!");
    }
}

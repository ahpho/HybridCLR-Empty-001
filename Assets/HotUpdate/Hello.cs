using System.Collections;
using UnityEngine;

public class Hello
{
    public static void Run()
    {
        Debug.Log("Hello, HybridCLR ! ========================================================================= ");

        /* 
           adb -s 127.0.0.1:16384 push HotUpdate.dll.bytes /sdcard/Android/data/com.Ksh.Hybrid001/files/Res/Assembly/
           �滻�����������󣬵���˵�HybridCLR/CompileDll/ActiveBuildTarget��Ȼ����������HotUpdate.dll.bytes�󣬿������ֻ�
        */
        //Debug.Log("Hello, HybridCLR ! ========================================================================= HotUpdated 1617!");
    }
}

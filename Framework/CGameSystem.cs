using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CGameSystem : MonoBehaviour, IGameSys
{
    public static CEventMgr EventMgr
    {
        get { return CGameRoot.GetGameSystem<CEventMgr>(); }
    }
  
    private string mName = null;
    public string Name
    {
        get 
        {
            if (mName == null)
                mName = GetType().Name;
            return mName; 
        }
    }

    public virtual void PreloadRes()
    {
        // override for preload resource
    }

    public EStateType EnableInState
    {
        get { return mEnableInState; }
        set { mEnableInState = value; }
    }
    private EStateType mEnableInState;

    public virtual SEventData[] GetEventMap()
    {
        return new SEventData[] { };
    }

    #region Initial

    public virtual void SysInitial()
    {
        SEventData[] eventMap = GetEventMap();
        for (int i = 0; i < eventMap.Length; ++i)
        {
            EventMgr.AddListener(eventMap[i].eventKey, eventMap[i].eventHandle, gameObject);
        }
    }

    #endregion

    #region Finalize

    public virtual void SysFinalize()
    {
        if (EventMgr == null) return;

        SEventData[] eventMap = GetEventMap();
        for (int i = 0; i < eventMap.Length; ++i)
        {
            EventMgr.DetachListener(eventMap[i].eventKey, eventMap[i].eventHandle);
        }
        //ClearSystem();
        //CloseUI();

    }


    public virtual void ClearSystem()
    {

    }
    //public virtual void CloseUI()
    //{
        
    //}


    #endregion

    #region Enter

    public bool SysEnabled { get { return mSysEnabled; } }
    protected bool mSysEnabled = false;
    public bool SysEntering { get { return mSysEntering; } }
    protected bool mSysEntering = false;

    public bool _SysEnter()
    {
        mSysEntering = true;
        return SysEnter();
    }

    /// <summary>
    /// 返回 TRUE 则执行 SysEnterCo
    /// </summary>
    /// <returns></returns>
    public virtual bool SysEnter()
    {
        return false;
    }

    public virtual IEnumerator SysEnterCo()
    {
        yield break;
    }

    public void _EnterFinish()
    {
        mSysEnabled = true;
        mSysEntering = false;
    }

    #endregion

    #region Leave

    public void _SysLeave()
    {
        mSysEnabled = false;
        EnableInState = EStateType.None;
        SysLeave();
    }

    public virtual void SysLeave()
    {
    }

    public virtual void SysLastLeave()
    {

    }

    #endregion

    public virtual void SysUpdate()
    {

    }

    public virtual void OnStateChangeFinish()
    {
    }

    #region HelpFun

   
    public static GameObject[] GetSelectGameObj()
    {
#if UNITY_EDITOR
        System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; ++i)
        {
            string name = assemblies[i].FullName.Split(',')[0];
            if (name == "Assembly-CSharp-Editor")
            {
                System.Type mEditorModeType = assemblies[i].GetType("CDebugToolsWindow");
                GameObject[] ret = (GameObject[])mEditorModeType.InvokeMember("GetSelectGameObj", System.Reflection.BindingFlags.InvokeMethod
                    | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
                    null, null, new object[] { });

                return ret;
            }
        }
        return null;
#else
        return null;
#endif
    }

    public static bool IsSelectGameObj(GameObject go)
    {
        GameObject[] select = CGameSystem.GetSelectGameObj();
        if (select != null)
        {
            for (int i = 0; i < select.Length; ++i)
            {
                if (select[i] == go)
                {
                    return true;
                }
            }
        }
        return false;
    }


    #endregion
}

public struct SEventData
{
    public ESysEvent eventKey;
    public DGameEventHandle eventHandle;

    public SEventData(ESysEvent eventKey, DGameEventHandle eventHandle)
    {
        this.eventKey = eventKey;
        this.eventHandle = eventHandle;
    }

    //public SEventData(MsgID protocolMsgID, DGameEventHandle eventHandle)
    //{
    //    this.eventKey = (int)protocolMsgID;
    //    this.eventHandle = eventHandle;
    //}

    //public SEventData(EEvent eventID, DGameEventHandle eventHandle)
    //{
    //    this.eventKey = (int)eventID;
    //    this.eventHandle = eventHandle;
    //}
}


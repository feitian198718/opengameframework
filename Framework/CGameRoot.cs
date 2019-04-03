#define DEBUG_LOG
//#define PROFILE

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif



public delegate void DGameStateChangeEventHandler(EStateType newState, EStateType oldState);

public interface IGameSys
{
}
[Serializable]
public class FGVersion
{
    public int Major = 0; // 主版本
    public int Minor = 0; // 次版本
    public int Patch = 0; // 补丁版本

    public FGVersion(string v)
    {
        var vs = v.Split('.');
        Major = int.Parse(vs[0]);
        Minor = int.Parse(vs[1]);
        if(vs.Length == 3)
            Patch = int.Parse(vs[2]);
    }
    public FGVersion(int major, int minor, int build)
    {
        Major = major;
        Minor = minor;
        Patch = build;
    }

    public override string ToString()
    {
        return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
    }

    public Version V
    {
        get { return new Version(Major, Minor, Patch); }
    }

    public static implicit operator FGVersion(Version v)
    {
        return new FGVersion(v.Major, v.Minor, v.Build);
    }

}
public class CGameRoot : MonoBehaviour
{
    #region EventTest
    /// <summary>
    /// 测试事件使用
    /// </summary>
    public string bEventTestName = "";
    #endregion

    public FGVersion Version = new FGVersion(0, 1, 0);

    public const string cRootName = "_GameRoot";

    //在开始一局新游戏的时候，总是会被随机重置
    private static int gameSeed;
    public static int RandomSeed
    {
        get
        {
            //Debug.Log("存储随机因子为：" + gameSeed);
            return gameSeed;
        }
        set
        {
            Debug.Log("设置随机因子为：" + value);
            gameSeed = value;
            UnityEngine.Random.InitState(gameSeed);
        }
    }

    static public event DGameStateChangeEventHandler OnStateChange;
    static public event DGameStateChangeEventHandler OnPreStateChange;
    static public event DGameStateChangeEventHandler OnPostStateChange;

    static private GameObject mInstanceObj;
    static public CGameRoot Instance
    {
        get
        {
            if(mInstanceObj == null)
            {
                mInstanceObj = GameObject.Find(cRootName);
                if (mInstanceObj == null) return null;
            }

            return mInstanceObj.GetComponent<CGameRoot>();
        }
    }

    static public bool IsInitialed
    {
        get
        {
            if (Instance == null) return false;
            else return Instance.mIsInitialed;
        }
    }

    static public void SwitchToState(EStateType stateType)
    {
        if(stateType == EStateType.None)
            return;
        Instance._SwitchToState(stateType);
    }

    static public T GetGameSystem<T>()
        where T : CGameSystem
    {
        if (Instance == null) return null;
        else return Instance._GetGameSystem<T>();
    }

    static public CGameSystem GetGameSystem(Type type)
    {
        if (Instance == null) return null;
        else return Instance._GetGameSystem(type);
    }

    static public bool HaveSystemRegisted(Type type)
    {
        return Instance._HaveSystemRegisted(type);
    }

    static public void PreloadRes()
    {
        Instance._PreloadRes();
    }

    static public EStateType PreState
    {
        get
        {
            if(Instance != null)
                return Instance.mPreState;
            else
                return EStateType.None;
        }
    }

    static public EStateType CurState
    {
        get
        {
            if(Instance != null)
                return Instance.mOldState;
            else
                return EStateType.None;
        }
    }

    public EGameRootCfgType mConfigType = EGameRootCfgType.Game;
    public EStateType mFirstStateName = EStateType.Initial; //默认初始化

    private CGameRootCfg mConfig;
    private EStateType mOldState = EStateType.None;
    private EStateType mPreState = EStateType.None;

    private bool mIsInitialed = false;

    private CGameSystem[] mSystems;
    private Dictionary<Type, CGameSystem> mSystemMap = new Dictionary<Type, CGameSystem>();

    public void Awake()
    {
        if (IsInitialed)
            return;
        CGameRootCfg.CreateCfg();

        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        mConfig =  CGameRootCfg.mCfgs[(int)mConfigType];
    }

    public void Start()
    {
        if (IsInitialed)
            return;


        //intial all system
        mSystems = mConfig.mInitialDelegate(transform);
        foreach(CGameSystem gameSys in mSystems)
        {
            mSystemMap.Add(gameSys.GetType(), gameSys);
        }
        foreach(CGameSystem gameSys in mSystems)
        {
            gameSys.SysInitial();
        }

        if(mFirstStateName != EStateType.None)
        {
            SwitchToState(mFirstStateName);
        }
        else
        {
            _SwitchToState(EStateType.Root);
        }

        mIsInitialed = true;
    }

    public void OnDestroy()
    {
        foreach(CGameSystem gameSys in mSystems)
        {
            gameSys.SysFinalize();
        }
    }

    public void OnClearData()
    {
        foreach(CGameSystem gameSys in mSystems)
        {
            gameSys.ClearSystem();
            //gameSys.CloseUI();
        }
    }

    public void Update()
    {
        if (mSystems == null) return;
        CGameSystem gameSystem;

        for(int i = 0; i < mSystems.Length; ++i)
        {

            gameSystem = mSystems[i];

            if(gameSystem.SysEnabled)
                gameSystem.SysUpdate();
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            //#if UNITY_ANDROID
            //            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            //            jo.Call("logout");
            //#endif
            //MessageBoxSys.SetMessageBox("是否退出游戏？", true, "确定", "取消", OnConfirm);
        }

        //NGUIDebug.Log(mSystems[index].name + " take time:" + maxtime);
        //rgba8888ToColor(ColorInt);
    }

    //public int ColorInt;
    //public float ColorR;
    //public float ColorG;
    //public float ColorB;
    //public float ColorA;

    //public void rgba8888ToColor(int value)
    //{
    //    ColorR = (((value & 0xFF000000) >> 24) / 255.0F);
    //    ColorG = (((value & 0xFF0000) >> 16) / 255.0F);
    //    ColorB = (((value & 0xFF00) >> 8) / 255.0F);
    //    ColorA = ((value & 0xFF) / 255.0F);
    //}

    #region 中断返回


#if UNITY_EDITOR
    /// <summary>
    /// 编辑器中启动时会调一次OnApplicationPause
    /// </summary>
    private bool mIsFirstInEditor = true;

#endif

    public bool IsPause
    {
        get { return mIsPause; }
    }

    private bool mIsPause = false;

    private bool mAwakingRuning = false;

    private void OnApplicationPause(bool pause)
    {

        if(mConfigType != EGameRootCfgType.Game)
            return;

#if UNITY_EDITOR
        if(mIsFirstInEditor)
        {
            mIsFirstInEditor = false;
            return;
        }
#endif

        mIsPause = pause;
        if(!mIsPause)
        {
            StartCoroutine(AwakeFromPauseCo());
        }
        else
        {
            if(mAwakingRuning)
            {
                StopCoroutine("AwakeFromPauseCo");
                mAwakingRuning = false;
            }

            //CEventMgr eventMgr = CGameRoot.GetGameSystem<CEventMgr>();
            //eventMgr.TriggerEvent(new CGameEvent((int)ESysEvent.GamePause));

            //CNetSys netSys = CGameRoot.GetGameSystem<CNetSys>();
            //netSys.SendHeartBeat();
        }


    }

    /// <summary>
    /// 中断返回后先把返回流程处理完
    /// </summary>
    private IEnumerator AwakeFromPauseCo()
    {
        using(CUsingHelper helper = new CUsingHelper(
            delegate() { mAwakingRuning = true; },
            delegate() { mAwakingRuning = false; }))
        {

            yield return null;

        }
    }

    #endregion

    #region Switch To State

    private Queue<EStateType> switchQueue = new Queue<EStateType>();

    private void _SwitchToState(EStateType newState)
    {
        switchQueue.Enqueue(newState);

        if(runing == false)
            StartCoroutine(HandleSwitchQueue());
    }

    bool runing = false;

    private IEnumerator HandleSwitchQueue()
    {
        runing = true;
        while(switchQueue.Count != 0)
        {
            yield return StartCoroutine(_SwitchToStateCo(switchQueue.Dequeue()));
        }
        runing = false;
    }

    private IEnumerator _SwitchToStateCo(EStateType newState)
    {
        if(mOldState == newState)
        {
            Debug.Log("SwitchState oldState == newState: " + newState);
            yield break;
        }

        if(OnPreStateChange != null)
            OnPreStateChange(newState, mOldState);

        CGameState[] oldStates = mConfig.mStateMap[mOldState];
        CGameState[] newStates = mConfig.mStateMap[newState];

        int sameDepth = -1;
        while(sameDepth + 1 < newStates.Length && sameDepth + 1 < oldStates.Length
            && newStates[sameDepth + 1] == oldStates[sameDepth + 1])
        {
            ++sameDepth;
        }

        List<CGameSystem> leaveSystems = new List<CGameSystem>();
        for(int i = oldStates.Length - 1; i > sameDepth; --i)
        {
            foreach(Type sysType in oldStates[i].mSystems)
            {
                leaveSystems.Add(mSystemMap[sysType]);
            }
        }

        foreach(CGameSystem leaveSystem in leaveSystems)
        {
            leaveSystem._SysLeave();
        }

        if(OnStateChange != null)
            OnStateChange(newState, mOldState);

        List<CGameSystem> enterSystems = new List<CGameSystem>();
        for(int i = sameDepth + 1; i < newStates.Length; ++i)
        {
            foreach(Type sysType in newStates[i].mSystems)
            {
                if(!mSystemMap.ContainsKey(sysType))
                    throw new Exception(string.Format("SystemMap.ContainsKey({0}) == false", sysType.Name));

                mSystemMap[sysType].EnableInState = newStates[i].mStateType;
                enterSystems.Add(mSystemMap[sysType]);
            }
        }


        //Debug.Log("555555555555555555555555....." + enterSystems.Count);
        for (int i = 0; i < enterSystems.Count; ++i)
        {
            CGameSystem enterSystem = enterSystems[i];
            bool haveEnterCo = enterSystem._SysEnter();
            if(haveEnterCo)
            {
                yield return StartCoroutine(enterSystem.SysEnterCo());
            }

            enterSystem._EnterFinish();
        }

        //加入了新系统之后，再给旧系统一次清理的机会。
        foreach(CGameSystem leaveSystem in leaveSystems)
        {
            leaveSystem.SysLastLeave();
        }

        foreach(CGameSystem enterSystem in enterSystems)
        {
            enterSystem.OnStateChangeFinish();
        }

        mPreState = mOldState;
        mOldState = newState;

        if(OnPostStateChange != null)
            OnPostStateChange(newState, mPreState);
    }

    #endregion

    private T _GetGameSystem<T>()
        where T : CGameSystem
    {
        if(mSystemMap.ContainsKey(typeof(T)))
            return (T)mSystemMap[typeof(T)];
        else
            return null;
    }

    private CGameSystem _GetGameSystem(Type type)
    {
        if(mSystemMap.ContainsKey(type))
            return mSystemMap[type];
        else
            return null;
    }

    private bool _HaveSystemRegisted(Type type)
    {
        return mSystemMap.ContainsKey(type);
    }

    // 这个暂时没有添加 不过应该添加到哪个节点 enter 和 enterfinsh之间？
    // 
    private void _PreloadRes()
    {
        for(int i = 0; i < mSystems.Length; ++i)
        {
            mSystems[i].PreloadRes();
        }
    }

    /*public CGameSystem GetCGameSystemByInterface(Type type)
    {
        if (mInterfaceMap.ContainsKey(type))
        {
            if (mSystems.ContainsKey(mInterfaceMap[type]))
                return mSystems[mInterfaceMap[type]];
            else
                return null;
        }
        else
        {
            return null;
        }
    }*/
    public void OnConfirm(object data)
    {
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("logout");
#endif
    }
}

public delegate void DVoid();

public class CUsingHelper : IDisposable
{
    private DVoid mOnDispose;

    public CUsingHelper(DVoid onCreate, DVoid onDispose)
    {
        if(onCreate != null)
            onCreate();

        mOnDispose = onDispose;
    }
    public void Dispose()
    {
        if(mOnDispose != null)
            mOnDispose();
    }
}

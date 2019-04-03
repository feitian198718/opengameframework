#define LOG_EVENT
//#define PROFILE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CEventMgr : CGameSystem
{
    private static CEventMgr _Instance = null;
    public static CEventMgr Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = CGameRoot.GetGameSystem<CEventMgr>();
            }   
            return _Instance;
        }
    }

    readonly string[] c_szErrorMessage = new string[]{
		"未知错误",
		"数据库出错",
		"帐号正在被使用中",
		"帐号已被禁用",
		"服务器维护中，请稍后登陆",
		"用户已满，请选择另一组服务器",
		"帐号待激活",

		"未知错误",
		"未知错误",
		"未知错误",
		"未知错误",
		"未知错误",

		"角色名重复",
		"用户档案不存在",
	};

    class CEventData
    {
        public bool mObjRelate;
        public DGameEventHandle mHandle;
        public GameObject mGameObj;

        public CEventData(DGameEventHandle handle)
        {
            mObjRelate = false;
            mHandle = handle;
            mGameObj = null;
        }

        public CEventData(DGameEventHandle handle, GameObject gameObj)
        {
            mObjRelate = true;
            mHandle = handle;
            mGameObj = gameObj;
        }
    }

    public bool mLimitQueueProcesing = false;
    public float mQueueProcessTime = 0.0f;

    private Dictionary<ESysEvent, List<CEventData>> mListenerTable = new Dictionary<ESysEvent, List<CEventData>>();
    private Queue mEventQueue = new Queue();

    //public bool AddNetListener(RockProtocol.MsgID msgID, DGameEventHandle eventHandle)
    //{
    //    return AddListener(GetNetMsgIdEventKey(msgID), eventHandle);
    //}

    //static public int GetNetMsgIdEventKey(RockProtocol.MsgID msgID)
    //{
    //    return (int)msgID;
    //}
    public bool RemoveListener(ESysEvent eventkey, DGameEventHandle eventHandle)
    {
        if(!mListenerTable.ContainsKey(eventkey))
            return false;

        List<CEventData> listenerList = mListenerTable[eventkey];
        for(int i = 0; i < listenerList.Count; ++i)
        {
            if (eventHandle == listenerList[i].mHandle)
            {
                listenerList.Remove(listenerList[i]);
                return true;
            }
        }
        return false;
    }
    public bool AddListener(ESysEvent eventkey, DGameEventHandle eventHandle)
    {
        if (eventHandle == null)
        {
            return false;
        }

        if (!mListenerTable.ContainsKey(eventkey))
            mListenerTable.Add(eventkey, new List<CEventData>());

        List<CEventData> listenerList = mListenerTable[eventkey];

        listenerList.Add(new CEventData(eventHandle));

        return true;
    }

    /// <summary>
    /// 为避免回调时被调方是一个已经被删除的GameObj上的组件，造成异常，组件需要使用此函数注册
    /// </summary>
    /// <param name="eventkey"></param>
    /// <param name="eventHandle"></param>
    /// <param name="gameObj"></param>
    /// <returns></returns>
    public bool AddListener(ESysEvent eventkey, DGameEventHandle eventHandle, GameObject gameObj)
    {
        if (eventHandle == null)
        {
            return false;
        }

        if (!mListenerTable.ContainsKey(eventkey))
            mListenerTable.Add(eventkey, new List<CEventData>());

        List<CEventData> listenerList = mListenerTable[eventkey];

        listenerList.Add(new CEventData(eventHandle, gameObj));

        return true;
    }

    //public bool DetachNetListener(RockProtocol.MsgID msgID, DGameEventHandle eventHandle)
    //{
    //    return DetachListener(GetNetMsgIdEventKey(msgID), eventHandle);
    //}

    public bool DetachListener(ESysEvent eventKey, DGameEventHandle eventHandle)
    {
        if (!mListenerTable.ContainsKey(eventKey))
            return false;

        List<CEventData> listenerList = mListenerTable[eventKey];

        CEventData find = null;
        foreach (CEventData evtData in listenerList)
        {
            if (evtData.mHandle == eventHandle)
            {
                find = evtData;
                break;
            }
        }

        if (find != null)
            listenerList.Remove(find);

        return true;
    }

    /// <summary>
    /// 同步事件触发
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public bool TriggerEvent(IEvent evt)
    {
#if PROFILE
        Profiler.BeginSample("TriggerEvent");
#endif
        bool ret = true;
        ESysEvent eventKey = (ESysEvent)evt.GetKey();


        if (!mListenerTable.ContainsKey(eventKey))
        {
            ret = false;
        }
        else
        {
            List<CEventData> listenerList = mListenerTable[eventKey];

            //防止在事件处理流程中改变事件列表
            List<CEventData> tmpList = new List<CEventData>(listenerList);
            for (int i = 0; i < tmpList.Count; ++i)
            //foreach (CEventData evtData in tmpList)
            {
                CEventData evtData = tmpList[i];

                if (evtData.mHandle != null && (!evtData.mObjRelate || evtData.mGameObj != null))
                {
#if PROFILE
                    Profiler.BeginSample(string.Format("HandleEvt: {0}, {1}", eventKey, evtData.mGameObj != null ? evtData.mGameObj.name : "null"));
#endif
                    evtData.mHandle(evt);


#if PROFILE
                    Profiler.EndSample();
#endif
                }
            }

            for (int i = 0; i < tmpList.Count; ++i)
            {
                if (tmpList[i].mObjRelate && tmpList[i].mGameObj == null)
                {
                    listenerList.Remove(tmpList[i]);
                }
            }
        }


        //改变事件等待协程的状态
        if (mEvtWaiterMap.ContainsKey(eventKey))
        {
            foreach (CEvtWaiter waiter in mEvtWaiterMap[eventKey])
            {
                waiter.mEvt = evt;
            }
            mEvtWaiterMap[eventKey].Clear();
            mEvtWaiterMap[eventKey] = null;
            mEvtWaiterMap.Remove(eventKey);
        }

#if PROFILE
        Profiler.EndSample();
#endif
        return ret;
    }

    public bool QueueEvent(IEvent evt)
    {
        mEventQueue.Enqueue(evt);
        return true;
    }

    public override void SysUpdate()
    {
        float timer = 0.0f;
        while (mEventQueue.Count > 0)
        {
            if (mLimitQueueProcesing)
            {
                if (timer > mQueueProcessTime)
                    return;
            }

            IEvent evt = mEventQueue.Dequeue() as IEvent;
            TriggerEvent(evt);

            if (mLimitQueueProcesing)
                timer += Time.deltaTime;
        }
    }

    public override void SysFinalize()
    {
        base.SysFinalize();
        mListenerTable.Clear();
        mEventQueue.Clear();
    }

    #region Wait Event

    Dictionary<ESysEvent, List<CEvtWaiter>> mEvtWaiterMap = new Dictionary<ESysEvent, List<CEvtWaiter>>();

    public IEnumerator WaitEvent(CEvtWaiter waiter)
    {
        if (mEvtWaiterMap.ContainsKey(waiter.mEvtKey))
        {
            mEvtWaiterMap[waiter.mEvtKey].Add(waiter);
        }
        else
        {
            mEvtWaiterMap.Add(waiter.mEvtKey, new List<CEvtWaiter>());
            mEvtWaiterMap[waiter.mEvtKey].Add(waiter);
        }

        float waitTime = waiter.mWaitTime;
        float startTime = Time.time;

        while (waiter.mEvt == null)
        {
            if (waiter.mWaitTime > 0
                && Time.time - startTime > waitTime)
            {
                waiter.mState = EEvtWaiterState.OutOfTime;
                yield break;
            }
            yield return null;
        }
        waiter.mState = EEvtWaiterState.Received;
    }

    #endregion


    bool StartGame()
    {


        return true;
    }

}

public enum EEvtWaiterState
{
    Waiting,
    Received,
    OutOfTime,
}


public class CEvtWaiter
{
    public const float cDefaultWaitTime = 10f;//20f

    /// <summary>
    /// 使用默认超时时间
    /// </summary>
    /// <param name="evtKey"></param>
    public CEvtWaiter(ESysEvent evtKey)
    {
        mEvtKey = evtKey;
        mWaitTime = cDefaultWaitTime;
        mEvt = null;
    }

    /// <summary>
    /// 自定义超时时间，-1表示不使用超时机制
    /// </summary>
    /// <param name="evtKey"></param>
    /// <param name="waitTime"></param>
    public CEvtWaiter(ESysEvent evtKey, float waitTime)
    {
        mEvtKey = evtKey;
        mWaitTime = waitTime;
        mEvt = null;
    }

    public ESysEvent mEvtKey;
    public float mWaitTime;
    public IEvent mEvt;
    public EEvtWaiterState mState = EEvtWaiterState.Waiting;
}

public delegate void GameEventDelegate(object data = null);
public class CGameEvent : IEvent
{
    private ESysEvent key;
    private object param1;
    private object param2;
    private GameEventDelegate paramFun;

    public CGameEvent(ESysEvent k, object p1 = null)
    {
        key = k;
        param1 = p1;
        param2 = null;
    }

    public CGameEvent(ESysEvent k, object p1, object p2)
    {
        key = k;
        param1 = p1;
        param2 = p2;
    }

    public CGameEvent(ESysEvent k)
    {
        key = k;
        param1 = null;
        param2 = null;
    }
    public CGameEvent(ESysEvent k, GameEventDelegate fun, object p1 = null)
    {
        key = k;
        param1 = p1;
        param2 = null;
        paramFun = fun;
    }

    public int GetKey()
    {
        return (int)key;
    }

    public object GetParam1()
    {
        return param1;
    }

    public object GetParam2()
    {
        return param2;
    }

    public GameEventDelegate GetFun()
    {
        return paramFun;
    }

}

//public class CNetEvent : IEvent
//{
//    RockProtocol.MsgID msgID;
//    RockProtocol.KylinMsg msg;

//    public RockProtocol.MsgBody MsgBody
//    {
//        get { return msg.stM_stMsgBody; }
//    }
//    public RockProtocol.MsgHead MsgHead
//    {
//        get { return msg.stM_stMsgHead; }
//    }

//    public CNetEvent(RockProtocol.MsgID msgID, RockProtocol.KylinMsg msg)
//    {
//        this.msgID = msgID;
//        this.msg = msg;
//    }

//    public int GetKey()
//    {
//        return CEventMgr.GetNetMsgIdEventKey(msgID);
//    }

//    public object GetParam1()
//    {
//        return msg;
//    }

//    public object GetParam2()
//    {
//        return null;
//    }

//}


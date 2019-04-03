using UnityEngine;
using System.Collections;
public delegate bool DGameEventHandle(IEvent evt);

public interface IEvent
{
    int GetKey();
    object GetParam1();
    object GetParam2();

    GameEventDelegate GetFun();

}

//public class CTriggerEvent : IEvent
//{
//    public ETriggerEvent triggerEvt;
//    public int[] param;
//    public uint senderUin;
//    public bool relAvatarOnly;
//    public uint avatarUin;

//    public CTriggerEvent(ETriggerEvent triggerEvt, uint senderUin, bool relAvatarOnly, uint avatarUin, int[] param)
//    {
//        this.triggerEvt = triggerEvt;
//        this.param = param;
//        this.senderUin = senderUin;
//        this.relAvatarOnly = relAvatarOnly;
//        this.avatarUin = avatarUin;
//    }

//    #region IEvent Members

//    public int GetKey()
//    {
//        return (int)triggerEvt;
//    }

//    public object GetParam1()
//    {
//        return senderUin;
//    }

//    public object GetParam2()
//    {
//        return param;
//    }

//    public GameEventDelegate GetFun()
//    {
//        return null;
//    }
//    #endregion
//}


//public class CSystemEvent : IEvent
//{

//    public ESysEvent mSysEvt;
//    public object mParam1;
//    public object mParam2;

//    public CSystemEvent(ESysEvent sysEvt)
//    {
//        mSysEvt = sysEvt;
//    }

//    public CSystemEvent(ESysEvent sysEvt, object param1, object param2)
//    {
//        this.mSysEvt = sysEvt;
//        this.mParam1 = param1;
//        this.mParam2 = param2; 
//    }

//    #region IEvent 成员

//    public int GetKey()
//    {
//        return (int)mSysEvt;
//    }

//    public object GetParam1()
//    {
//        return mParam1;
//    }

//    public object GetParam2()
//    {
//        return mParam2;
//    }

//    #endregion
//}


//public class CNetSysEvent : IEvent
//{
//    public int mEventId;
//    public int mgsParamType;
//    //public CSMsgResponse mMsg;
//    public byte[] mMsg;

//    //public CNetSysEvent(int EventId, CSMsgResponse msg)
//    //public CNetSysEvent(int EventId, stNullClientCmd msg)
//    //{
//    //    this.mEventId = EventId;
//    //    mMsg = msg;
//    //}

//    public CNetSysEvent(int gsCmd, int gsParamType, byte[] msg)
//    {
//        this.mEventId = gsCmd;
//        mgsParamType = gsParamType;
//        mMsg = msg;
//    }


//    #region IEvent 成员

//    public int GetKey()
//    {
//        return mEventId;
//    }

//    public object GetParam1()
//    {
//        return mMsg;
//    }

//    public object GetParam2()
//    {
//        return null;
//    }

//    #endregion
//}
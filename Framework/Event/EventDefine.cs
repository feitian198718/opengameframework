using UnityEngine;
using System.Collections;

public enum ESysEvent
{
    None,
    ShowFightUI,
    ChangeHP,
    CardHurtPlayer,

    Settlement,
    BossSettlement,

    ChangeMoney,

    GetRelic,
    RemoveRelic,

    DoThingsToCard,

    PlotStepOver,
}


/// <summary>
/// 触发事件定义
/// 因为涉及到美术的编辑，值定好之后不可变更
/// </summary>
public enum ETriggerEvent
{
    TriggerEvtStart = 40000,
    TestPressAlpha7ToTrigger = 40001,   //测试事件，按键7触发
    TestPressAlpha8ToTrigger = 40002,   //测试事件，按键8触发
    //OperMistake = 40004,            //操作失误的时候产生
    //BeginPlay = 40005,              //玩家在待机状态下操作正确时产生
    //ComboLevelChange = 40006,       //玩家的Combo等级改变时
    //GameStart = 40010,              //游戏开始时触发
    //ComboRateChange = 40011,        //Combo数占所有combo数的百分比，每改变0.05触发一次   
    ShowTimeBegin = 40012,            //ShowTime开始
    ShowTimeEnd = 40013,              //ShowTime结束
    ComboEffect = 40014,        //ComboEffect
    VoiceOneTeacherTurn = 40015,    //一个导师转身效果
    VoiceAllTeacherTurn = 40016,       //所有导师转身效果
    EffectGameEnd = 40017,           //唱歌结束   关闭所有角色的特效   

    Behaviour = 40018,              //行为的触发
    BehaviourEffect = 40019,        //由行为代理触发的角色特效
    MicrophoneBegin = 40020,        //打开麦克风
    MicrophoneEnd = 40021,          //关闭麦克风
    //-----------------------------------------------------------
    //根据歌曲编辑的事件触发，在SongArtEditor中编辑触发时间
    SongArtPartStart = 41000,
    ClimaxBegin = 41001,            //高潮开始
    ClimaxEnd = 41002,              //高潮结束

    SongArtPartEnd,
    //-----------------------------------------------------------
}


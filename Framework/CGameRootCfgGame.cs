//#define UNITY_ONLY //移除unity之外的组件，用来测内存占用

using UnityEngine;
using System.Collections;
using System;


partial class CGameRootCfg
{
    public static CGameRootCfg mGame = new CGameRootCfg(

    #region 各系统初始化顺序定义
    delegate (Transform rootObj)
    {
        return new CGameSystem[]
                {

                     CreateGameSys<CEventMgr>(rootObj),              //事件系统
                    
                };

    },
    #endregion

    #region 状态定义 & 状态下各系统的定义
    /*
     root
    /    \

     */
     new CGameState(EStateType.Root,
            new Type[]
            {
                

            },
            new CGameState[]
            {
                new CGameState(EStateType.Initial,
                    new Type[]
                    {
                    }, null),
                
                new CGameState(EStateType.Login,
                    new Type[]
                    {
                        

                    }, null
                ),

                new CGameState(EStateType.Home,
                    new Type[]
                    {
                      
                    }, null
                ),

                new CGameState(EStateType.Battle,
                    new Type[]
                    {
                       
                    }, null
                ),
                
                new CGameState(EStateType.ReBattle,
                    new Type[]
                    {
                       
                    }, null
                )
                
            }
        )

    );
    #endregion



}

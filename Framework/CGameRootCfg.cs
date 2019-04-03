using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum EStateType
{
    None,
    Root,
    Initial,
    Login,
	Battle,
    ReBattle,
    Home,

}

class CGameState
{
    public EStateType mStateType;
    public Type[] mSystems;
    public CGameState[] mChildState;

    public CGameState(EStateType stateType, Type[] systems, CGameState[] childState)
    {
        mStateType = stateType;
        mSystems = systems;
        mChildState = childState;
    }
}

public enum EGameRootCfgType
{
    None = 0,
    Game,
    Loading,
    NpcTalk,
    TestUI,
    SkillEditor,
}

public delegate CGameSystem[] DInitialSysDelegate(Transform rootObj);

partial class CGameRootCfg
{
    public static CGameRootCfg[] mCfgs = new CGameRootCfg[]
    {
        null,
        mGame,
        mLoading,

    };

    public static void CreateCfg()
    {
        mCfgs[0] = null;
        mCfgs[1] = mGame;
        mCfgs[2] = mLoading;
    }


    public CGameState mRoot;
    public DInitialSysDelegate mInitialDelegate;
    public Dictionary<EStateType, CGameState[]> mStateMap = new Dictionary<EStateType, CGameState[]>();


   
    public CGameRootCfg(DInitialSysDelegate initialSysDelegate, CGameState root)
    {
        mInitialDelegate = initialSysDelegate;
        mRoot = root;

        List<CGameState> parentList = new List<CGameState>();
        mStateMap.Clear();
        mStateMap.Add(EStateType.None, new CGameState[] { });
        BuildStateMap(mRoot, parentList);
    }

    private void BuildStateMap(CGameState state, List<CGameState> parentList)
    {
        if (mStateMap.ContainsKey(state.mStateType))
        {
            Debug.LogError(string.Format("same state already defined {0}", state.mStateType));
            throw new System.Exception();
        }

        parentList.Add(state);

        mStateMap.Add(state.mStateType, parentList.ToArray());

        if (state.mChildState != null)
        {
            foreach (CGameState childState in state.mChildState)
            {
                BuildStateMap(childState, parentList);
            }
        }

        parentList.RemoveAt(parentList.Count - 1);
    }


    private static CGameSystem CreateGameSys<T>(Transform rootObj)
    where T : CGameSystem
    {
        GameObject go = new GameObject();
        go.transform.parent = rootObj;
        CGameSystem ret = go.AddComponent<T>();
        go.name = ret.Name;
        return ret;
    }
}

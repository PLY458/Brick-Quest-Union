using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class GameState : ISceneState
{
    public GameState(SceneStateController Controller) : base(Controller)
    {
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TDG_game
{
        public class GameMgr : MonoBehaviour
    {
        [SerializeField]
        Vector2Int boardSize = new Vector2Int(11, 11);

        [SerializeField]
        GameBoard board = default;

        void Awake()
        {
            board.Initialize(boardSize);
        }

        void OnValidate()
        {
            if (boardSize.x < 2)
            {
                boardSize.x = 2;
            }
            if (boardSize.y < 2)
            {
                boardSize.y = 2;
            }
        }
    }


}

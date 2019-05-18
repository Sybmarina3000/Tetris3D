﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvent
{
    //кость остановилась
    public const string PLAY_GAME = "PLAY_GAME"; 
    public const string REPEAT_GAME = "REPEAT_GAME";
    public const string END_GAME = "END_GAME";
    public const string WIN_GAME = "WIN_GAME";

    public const string UI_PLAY = "UI_PLAY";

    public const string CURRENT_HEIGHT = "CURRENT_HEIGHT";

    public const string DESTROY_LAYER = "DESTROY_LAYER";
    public const string CURRENT_SCORE = "CURRENT_SCORE";


    public const string MOVE = "MOVE";
    public const string TURN = "TURN";

    public const string CHANGE_TIME_DROP = "CHANGE_TIME_DROP";

    public const string GAME_OVER = "GAME_OVER";

    // AFTER Decomposition
    public const string CREATE_NEW_ELEMENT = "CREATE_NEW_ELEMENT";
    public const string TURN_ELEMENT = "TURN_ELEMENT";
    public const string MOVE_ELEMENT = "MOVE_ELEMENT";
    //

}

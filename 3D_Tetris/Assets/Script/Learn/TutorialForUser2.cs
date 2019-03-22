﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialForUser2 : Listener<turn>
{

    static private float time = 0.2f;
    [Header("TURN")]
    [SerializeField] List<GameObject> CheckMotion = new List<GameObject>(Enum.GetValues(typeof(turn)).Length);

    [SerializeField] UnityEvent FinishLearn;

    private void Awake()
    {
        CallDelegate = ListenEvent;
    }
    public void ListenEvent(turn param)
    {
        Debug.Log("I KNIW TURN");
        CheckMotion[(int)param].SetActive(true);
        if (CheckMotion.Where(s => s.active).ToArray().Length == CheckMotion.Count)
        {
            FinishLearn.Invoke();
        }

    }

    public void HideAfterWait(GameObject obj)
    {
        StartCoroutine(Tools.SetActiveAfterWait(obj, time, false));
    }

    public void OpenAfterWait(GameObject obj)
    {
        StartCoroutine(Tools.SetActiveAfterWait(obj, time, true));
    }
}
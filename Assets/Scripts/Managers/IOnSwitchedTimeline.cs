using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnSwitchedTimeline
{
    void OnPlayerSwitched();
    void OnOtherPlayerSwitched(bool firstTime = false);
}

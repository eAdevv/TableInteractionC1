using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableInteraction.CoreLoop;

namespace TableInteraction.Events
{
    public static class EventManager 
    {
        public delegate float OnGetSpeedHandler();
        public static OnGetSpeedHandler OnGetSpeed;

        public delegate void CharacterEnterQueueHandler(Character character);
        public static CharacterEnterQueueHandler OnCharacterEnterQueue;

    }
}

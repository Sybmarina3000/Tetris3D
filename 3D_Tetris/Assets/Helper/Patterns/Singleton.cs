﻿using UnityEngine;

namespace Helper.Patterns
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        public static T Instance { get; private set; }

        private void Awake() {
            if (Instance == null) {
                Instance = this as T;

                Init();
            }
            else
                Destroy(this);
        }

        /// <summary>
        /// Если нужен Awake() в дочернем классе.
        /// </summary>
        protected virtual void Init() {
        }
    }
}
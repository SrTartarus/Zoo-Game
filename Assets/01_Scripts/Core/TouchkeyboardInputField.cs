using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Zoo.Core
{
    public class TouchkeyboardInputField : InputField
    {
        public class KeyboardDoneEvent : UnityEvent { }

        [SerializeField]
        private KeyboardDoneEvent m_keyboardDone = new KeyboardDoneEvent();

        public KeyboardDoneEvent onKeyboardDone
        {
            get { return m_keyboardDone; }
            set { m_keyboardDone = value; }
        }

        // Update is called once per frame
        void Update()
        {
            if(m_Keyboard != null && m_Keyboard.status == TouchScreenKeyboard.Status.Done && m_Keyboard.status != TouchScreenKeyboard.Status.Canceled)
            {
                m_keyboardDone.Invoke();
            }
        }
    }
}
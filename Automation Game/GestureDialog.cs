using Android.App;
using Android.Content;
using Android.Views;
using System;
using System.Collections.Generic;

namespace Automation_Game
{
    class GestureDialog : Dialog
    {
        List<Action<MotionEvent>> onTouchEvents;
        public GestureDialog(Context context) : base(context)
        {
            onTouchEvents = new List<Action<MotionEvent>>();
        }

        public void AddOnTouchEvent(Action<MotionEvent> func)
        {
            onTouchEvents.Add(func);
        }

        public void RemoveOnTouchEvent(Action<MotionEvent> func)
        {
            for (int i = 0; i < onTouchEvents.Count; i++)
            {
                if (onTouchEvents[i] == func)
                {
                    onTouchEvents.RemoveAt(i);
                    return;
                }
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            for (int i = 0; i < onTouchEvents.Count; i++)
            {
                onTouchEvents[i].Invoke(e);
            }
            return true;
        }
    }
}
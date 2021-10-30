using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Automation_Game
{
    public class ScrollView : LinearLayout
    {
        public ScrollView(Context context, Orientation orientation) : base(context)
        {
            Orientation = orientation;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Move)
            {
                if (e.HistorySize > 0)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        View child = GetChildAt(ChildCount - 1);
                        int i = 0;
                        bool CanScroll = child.GetY() + child.Height + e.GetY() - e.GetHistoricalY(i) >= Height;
                        if (!CanScroll)
                        {
                            i = e.HistorySize - 1;
                            CanScroll = child.GetY() + child.Height + e.GetY() - e.GetHistoricalY(i) >= Height;
                        }
                        bool CanScroll1 = CanScroll && GetChildAt(0).GetY() + e.GetY() - e.GetHistoricalY(i) <= 0;
                        for (int n = 0; n < ChildCount && CanScroll1; n++)
                        {
                            View v = GetChildAt(n);
                            float before = v.GetY();
                            v.SetY(v.GetY() + e.GetY() - e.GetHistoricalY(0));
                            float after = v.GetY();
                            Invalidate();
                        }
                    }
                    else
                    {
                        View child = GetChildAt(ChildCount - 1);
                        int i = 0;
                        bool CanScroll = child.GetX() + child.Width + e.GetX() - e.GetHistoricalX(i) >= Width;
                        if (!CanScroll)
                        {
                            i = e.HistorySize - 1;
                            CanScroll = child.GetX() + child.Width + e.GetX() - e.GetHistoricalX(i) >= Width;
                        }
                        bool CanScroll1 = CanScroll && GetChildAt(0).GetX() + e.GetX() - e.GetHistoricalX(i) <= 0;
                        for (int n = 0; n < ChildCount && CanScroll1; n++)
                        {
                            View v = GetChildAt(n);
                            float before = v.GetX();
                            v.SetX(v.GetX() + e.GetX() - e.GetHistoricalX(0));
                            float after = v.GetX();
                            Invalidate();
                        }
                    }
                }
            }
            return true;
        }

    }
}
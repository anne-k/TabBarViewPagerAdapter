using System;
using Android.OS;

namespace Droid.Source.Common.Tabs
{
    public sealed class ViewPagerFragmentInfoParcelableCreator : Java.Lang.Object, IParcelableCreator
    {
        public Java.Lang.Object CreateFromParcel (Parcel source)
        {
            return new ViewPagerFragmentInfoParcelable (source);
        }

        public Java.Lang.Object[] NewArray (int size)
        {
            return new ViewPagerFragmentInfoParcelable[size];
        }
    }
}

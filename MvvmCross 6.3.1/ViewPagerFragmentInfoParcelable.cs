using System;
using Android.OS;
using Java.Interop;

namespace Droid.Source.Common.Tabs
{
    public class ViewPagerFragmentInfoParcelable : Java.Lang.Object, IParcelable
    {
        public Type FragmentType;
        public Type ViewModelType;
        public string Title;
        public string Tag;

        [ExportField ("CREATOR")]
        public static ViewPagerFragmentInfoParcelableCreator InititalizeCreator ()
        {
            return new ViewPagerFragmentInfoParcelableCreator ();
        }

        public ViewPagerFragmentInfoParcelable ()
        {
        }

        public ViewPagerFragmentInfoParcelable (Parcel source)
        {
            string fragmentType = source.ReadString ();
            string viewModelType = source.ReadString ();
            Title = source.ReadString ();
            Tag = source.ReadString ();

            FragmentType = Type.GetType (fragmentType);
            ViewModelType = Type.GetType (viewModelType);
        }

        public void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteString (FragmentType.AssemblyQualifiedName);
            dest.WriteString (ViewModelType.AssemblyQualifiedName);
            dest.WriteString (Title);
            dest.WriteString (Tag);
        }

        public int DescribeContents ()
        {
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Java.Lang;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;

namespace Droid.Source.Common.Tabs
{
    public class TabBarViewPagerAdapter : MvxCachingFragmentStatePagerAdapter
    {
        private const string _bundleFragmentsInfoKey = "TabStatePagerAdapter_FragmentsInfo";
        private FragmentManager _fragmentManager;

        protected TabBarViewPagerAdapter (IntPtr javaReference, JniHandleOwnership transfer)
            : base (javaReference, transfer)
        {
        }

        public TabBarViewPagerAdapter (Context context, FragmentManager fragmentManager,
            List<MvxViewPagerFragmentInfo> fragmentsInfo) : base (context, fragmentManager, fragmentsInfo)
        {
            _fragmentManager = fragmentManager;
        }

        public override IParcelable SaveState ()
        {
            var bundle = base.SaveState () as Bundle;
            SaveFragmentsInfoState (bundle);
            return bundle;
        }

        public override void RestoreState (IParcelable state, ClassLoader loader)
        {
            base.RestoreState (state, loader);

            if (state is Bundle bundle) {
                RestoreFragmentsInfoState (bundle);
            }
        }

        public override Fragment GetItem (int position, Fragment.SavedState fragmentSavedState = null)
        {
            var fragment = base.GetItem (position, fragmentSavedState);

            // If the MvxViewPagerFragmentInfo for this position doesn't have the ViewModel, overwrite it with a new MvxViewPagerFragmentInfo that has the ViewModel we just created.
            // Not doing this means the ViewModel gets recreated every time the Fragment gets recreated!
            if (FragmentsInfo != null && FragmentsInfo.Count > position && fragment is IMvxFragmentView mvxFragment && mvxFragment.ViewModel != null) {

                var oldFragInfo = FragmentsInfo[position];

                if (oldFragInfo != null && !(oldFragInfo.Request is MvxViewModelInstanceRequest)) {
                    var viewModelInstanceRequest = new MvxViewModelInstanceRequest (mvxFragment.ViewModel);
                    var newFragInfo = new MvxViewPagerFragmentInfo (oldFragInfo.Title, oldFragInfo.Tag, oldFragInfo.FragmentType, viewModelInstanceRequest);
                    FragmentsInfo[position] = newFragInfo;
                }
            }

            return fragment;
        }

        private void SaveFragmentsInfoState (Bundle bundle)
        {
            if (bundle == null || FragmentsInfo == null || FragmentsInfo.Count == 0) {
                return;
            }

            var fragmentInfoParcelables = new IParcelable[FragmentsInfo.Count];
            int i = 0;

            foreach (var fragInfo in FragmentsInfo) {
                var parcelable = new ViewPagerFragmentInfoParcelable  {
                    FragmentType = fragInfo.FragmentType,
                    ViewModelType = fragInfo.Request.ViewModelType,
                    Title = fragInfo.Title,
                    Tag = fragInfo.Tag
                };
                fragmentInfoParcelables[i] = parcelable;
                i++;
            }

            bundle.PutParcelableArray (_bundleFragmentsInfoKey, fragmentInfoParcelables);
        }

        private void RestoreFragmentsInfoState (Bundle bundle)
        {
            if (bundle == null) {
                return;
            }

            var fragmentInfoParcelables = bundle.GetParcelableArray (_bundleFragmentsInfoKey);

            if (fragmentInfoParcelables == null) {
                return;
            }

            // First, we create a list of the ViewPager fragments that were restored by Android.
            var fragments = GetFragmentsFromBundle (bundle);

            // Now we get the FragmentInfo data for each fragment from the bundle.
            int i = 0;
            foreach (ViewPagerFragmentInfoParcelable parcelable in fragmentInfoParcelables) {
                MvxViewPagerFragmentInfo fragInfo = null;

                if (i < fragments.Count) {
                    var f = fragments[i];
                    if (f is IMvxFragmentView fragment && fragment.ViewModel != null) {
                        // The fragment was already restored by Android with its old ViewModel (cached by MvvmCross).
                        // Add the ViewModel to the FragmentInfo object so the adapter won't instantiate a new one.
                        var viewModelInstanceRequest = new MvxViewModelInstanceRequest (fragment.ViewModel);
                        fragInfo = new MvxViewPagerFragmentInfo (parcelable.Title, parcelable.Tag, parcelable.FragmentType, viewModelInstanceRequest);
                    }
                }

                if (fragInfo == null) {
                    // Either the fragment doesn't exist or it doesn't have a ViewModel. 
                    // Fall back to a FragmentInfo with the ViewModelType. The adapter will create a ViewModel in GetItem where we will add it to the FragmentInfo.
                    var viewModelRequest = new MvxViewModelRequest (parcelable.ViewModelType);
                    fragInfo = new MvxViewPagerFragmentInfo (parcelable.Title, parcelable.Tag, parcelable.FragmentType, viewModelRequest);
                }

                FragmentsInfo.Add (fragInfo);
                i++;
            }

            NotifyDataSetChanged ();
        }

        private List<Fragment> GetFragmentsFromBundle (Bundle bundle)
        {
            var fragments = new List<Fragment> ();
            if (bundle == null || _fragmentManager == null || _fragmentManager.Fragments == null) {
                return fragments;
            }

            // This is how the base adapter retrieves its fragments from the bundle.
            // Copy-pasted here because the base adapter's fragment list is private
            var keys = bundle.KeySet ();
            foreach (var key in keys) {
                if (!key.StartsWith ("f"))
                    continue;

                var index = Integer.ParseInt (key.Substring (1));

                if (_fragmentManager.Fragments == null) return fragments;

                var f = _fragmentManager.GetFragment (bundle, key);
                if (f != null) {
                    while (fragments.Count () <= index)
                        fragments.Add (null);

                    fragments[index] = f;
                }
            }

            return fragments;
        }
    }
}

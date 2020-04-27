# TabBarViewPagerAdapter

Custom ViewPagerAdapter inheriting from MvxCachingFragmentStatePagerAdapter. The sole purpose of this adapter is to save and restore the FragmentsInfo property (Mvx doesn't currently do this).

Two versions, depending on the MvvmCross version it is compatible with:

- MvvmCross 6.3.1 - may work for older versions, too. Will break with MvvmCross 6.4.0
- MvvmCross 6.4 - created with 6.4.1, may break with future versions.

## Using the Adapter

If your ViewPager is in an Activity, add this to the Activity:

```c# 
protected override void OnPostCreate (Bundle savedInstanceState)
{
    base.OnPostCreate (savedInstanceState);

    if (_viewPager == null) {
        return;
    }

    if (!(_viewPager.Adapter is TabBarViewPagerAdapter)) {
        _viewPager.Adapter = new TabBarViewPagerAdapter (this, this.SupportFragmentManager, new List<MvxViewPagerFragmentInfo> ());
    }
}
```


If your ViewPager is in a Fragment, add this to the Fragment:

```c#
public override void OnViewCreated (View view, Bundle savedInstanceState)
{
    base.OnViewCreated (view, savedInstanceState);

    if (_viewPager == null) {
        return;
    }

    if (!(_viewPager.Adapter is TabBarViewPagerAdapter)) {
        _viewPager.Adapter = new TabBarViewPagerAdapter (this.Context, this.ChildFragmentManager, new List<MvxViewPagerFragmentInfo> ());
    }
}
```

## OffscreenPageLimit and restoring ViewModels

A ViewPager only keeps a limited number of Fragments in memory. How many is determined by the value of the OffscreenPageLimit property. The default value of this property is 1, which means only the current page, the previous page and the next page will be alive.

In a save&restore cycle, ViewModels for pages that are currently alive will be saved by MvvmCross. However, ViewModels for pages that are outside of the OffscreenPageLimit will be lost. The adapter will create a new ViewModel the first time the user navigates to that page again, and will keep using that ViewModel for the remainder of the session.

If you need the ViewModels for all of your pages to persist through save&restore, make sure to set the ViewPager.OffscreenPageLimit to a number that will keep all of your pages alive at all times. Only do this if you have a small amount of pages, or performance will suffer. See the Android docs: https://developer.android.com/reference/android/support/v4/view/ViewPager#setoffscreenpagelimit



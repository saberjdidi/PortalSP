using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamarinApplication;
using XamarinApplication.iOS;

[assembly: ExportRenderer(typeof(CustomListview), typeof(CustomListViewRenderer))]
namespace XamarinApplication.iOS
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.ShowsVerticalScrollIndicator = false;
            }
        }
    }
}
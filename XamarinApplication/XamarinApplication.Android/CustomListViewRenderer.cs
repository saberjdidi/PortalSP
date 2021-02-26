using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinApplication;
using XamarinApplication.Droid;

[assembly: ExportRenderer(typeof(CustomListview), typeof(CustomListViewRenderer))]
namespace XamarinApplication.Droid
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        Context _context;
        public CustomListViewRenderer(Context context) : base(context)
        {
            _context = context;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.VerticalScrollBarEnabled = false;
            }
        }
    }
}
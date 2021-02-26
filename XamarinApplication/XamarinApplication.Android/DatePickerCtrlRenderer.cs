using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinApplication.Converters;
using XamarinApplication.Droid;

[assembly: ExportRenderer(typeof(DatePickerCtrl), typeof(DatePickerCtrlRenderer))]
namespace XamarinApplication.Droid
{
    [Obsolete] //~ ajouter cette 
    public class DatePickerCtrlRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);
            this.Control.SetTextColor(Android.Graphics.Color.Rgb(83, 63, 149));
            this.Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            this.Control.SetPadding(20, 0, 0, 0);

            GradientDrawable gd = new GradientDrawable();
            gd.SetCornerRadius(25); //increase or decrease to changes the corner look  
            gd.SetColor(Android.Graphics.Color.Transparent);
            gd.SetStroke(3, Android.Graphics.Color.Rgb(83, 63, 149));

            this.Control.SetBackgroundDrawable(gd);

            DatePickerCtrl element = Element as DatePickerCtrl;

            if (!string.IsNullOrWhiteSpace(element.Placeholder))
            {
                Control.Text = element.Placeholder;
            }

            this.Control.TextChanged += (sender, arg) => {
                var selectedDate = arg.Text.ToString();
                if (selectedDate == element.Placeholder)
                {
                    Control.Text = DateTime.Now.ToString("MM/dd/yyyy");
                }
            };
        }
    }
}
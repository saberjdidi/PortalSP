using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamarinApplication.Converters;
using XamarinApplication.iOS;

[assembly: ExportRenderer(typeof(DatePickerCtrl), typeof(DatePickerCtrlRenderer))]
namespace XamarinApplication.iOS
{
    public class DatePickerCtrlRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null)
                return;
            var element = e.NewElement as DatePickerCtrl;
            if (!string.IsNullOrWhiteSpace(element.Placeholder))
            {
                Control.Text = element.Placeholder;
            }
            Control.BorderStyle = UITextBorderStyle.RoundedRect;
            Control.Layer.BorderColor = UIColor.FromRGB(83, 63, 149).CGColor;
            Control.Layer.CornerRadius = 10;
            Control.Layer.BorderWidth = 1f;
            Control.AdjustsFontSizeToFitWidth = true;
            Control.TextColor = UIColor.FromRGB(83, 63, 149);

            Control.ShouldEndEditing += (textField) => {
                var seletedDate = (UITextField)textField;
                var text = seletedDate.Text;
                if (text == element.Placeholder)
                {
                    Control.Text = DateTime.Now.ToString("MM/dd/yyyy");
                }
                return true;
            };
        }

        private void OnCanceled(object sender, EventArgs e)
        {
            Control.ResignFirstResponder();
        }

        private void OnDone(object sender, EventArgs e)
        {
            Control.ResignFirstResponder();
        }
    }
}
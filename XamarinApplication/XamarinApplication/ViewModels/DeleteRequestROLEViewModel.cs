using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class DeleteRequestROLEViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Properties
        public Attachment Attachment { get; set; }
        public string Reason { get; set; }
        #endregion

        #region Constructors
        public DeleteRequestROLEViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Methods
        public async void deleteRequest()
        {
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            var deleteRequest = new DeleteRequest
            {
                id = Attachment.requests.Select(r => r.id).FirstOrDefault(),
                reason = Reason
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<DeleteRequest>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/request/delete",
            res,
            deleteRequest);
            /*if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            //RequestsViewModel.GetInstance().Update(deleteRequest);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Request Deleted");
            //await App.Current.MainPage.Navigation.PopPopupAsync(true);
            await PopupNavigation.Instance.PopAsync(true);
        }
        #endregion

        #region Commands
        public ICommand DeleteRequest
        {
            get
            {
                return new Command(() =>
                {
                    deleteRequest();
                });
            }
        }
        #endregion
    }
}

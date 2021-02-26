using GalaSoft.MvvmLight.Command;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class NewBranchViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private ImageSource imageSource;
        private MediaFile file;
        #endregion

        #region Constructor
        public NewBranchViewModel()
        {
            apiService = new ApiServices();
            this.ImageSource = "noImage";

        }
        #endregion

        #region Properties
        public string Code { get; set; }
        public string Name { get; set; }
        public ImageSource ImageSource
        {
            get => this.imageSource;
            set => this.SetValue(ref this.imageSource, value);
        }

        private bool value = false;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddNewBranch()
        {
            Value = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Name))
            {
                Value = true;
                return;
            }
            string imagePath = file.Path;
            string filename = imagePath.Substring(imagePath.LastIndexOf("/") + 1);
            string fileWOExtension;
            if (filename.IndexOf(".") > 0)
            {
                fileWOExtension = filename.Substring(0, filename.LastIndexOf("."));
            }
            else
            {
                fileWOExtension = filename;
            }
            var content = new MultipartFormDataContent();
            content.Headers.ContentType.MediaType = "multipart/form-data";
            content.Add(new StreamContent(file.GetStream()),
                "\"imageFile\"",
                $"\"{filename}\"");
            var _branch = new AddBranch
            {
                code = Code,
                name = Name,
                isLaboratory = true
            };
            var addBranch = new AddBranchImage
            {
                branch = _branch,
                fileData = content,
                extension = "jpg",
                name = fileWOExtension
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddBranchImage>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/patient/save",
            res,
            addBranch);
            /*if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Branch Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        /* public async void UploadFile()
         {
             Value = true;
             var connection = await apiService.CheckConnection();

             if (!connection.IsSuccess)
             {
                 await Application.Current.MainPage.DisplayAlert(
                     Languages.Warning,
                     Languages.CheckConnection,
                     Languages.Ok);
                 return;
             }
             if (file == null)
             {
                 Value = true;
                 await Application.Current.MainPage.DisplayAlert("Alert", "Please Select Image", "ok");
                 return;
             }

             string imagePath = file.Path;
             string filename = imagePath.Substring(imagePath.LastIndexOf("/") + 1);
             string fileWOExtension;
             if (filename.IndexOf(".") > 0)
             {
                 fileWOExtension = filename.Substring(0, filename.LastIndexOf("."));
             }
             else
             {
                 fileWOExtension = filename;
             }

             var content = new MultipartFormDataContent();
             content.Headers.ContentType.MediaType = "multipart/form-data";
             content.Add(new StreamContent(file.GetStream()),
                 "\"imageFile\"",
                 $"\"{filename}\"");
             //$"\"{file.Path.Substring(66)}\"");
             content.Add(new StringContent(Exercice.id.ToString()), "idExercise");

             var accesstoken = Settings.AccessToken;
             var client = new HttpClient();

             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
             var url = "http://phoneofficine.it/niini-gim/pictureExercise/save";
             var response = await client.PostAsync(url, content);
             Debug.WriteLine("********response*************");
             Debug.WriteLine(response);
             var result = await response.Content.ReadAsStringAsync();
             Debug.WriteLine("********result*************");
             Debug.WriteLine(result);
             if (!response.IsSuccessStatusCode)
             {
                 await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                 return;
             }
             if (response.IsSuccessStatusCode)
             {
                 Value = false;

                 if (Exercice.speed == null || Exercice.lood == null || Exercice.secondLood == null || Exercice.loodEccentric == null || Exercice.secondLoodEccentric == null || Exercice.eccentric == null || Exercice.concentric == null)
                 {
                     // await Application.Current.MainPage.DisplayAlert("Error", result, "ok");
                     DependencyService.Get<INotification>().CreateNotification("GYM TECH", "Image Uploaded");
                     await Navigation.PopPopupAsync();
                     return;
                 }

                 var newImage = JsonConvert.DeserializeObject<ImagesExercice>(result);

                 ImagesExerciceViewModel.GetInstance().AddImage(newImage);
                 DependencyService.Get<INotification>().CreateNotification("GYM TECH", "Image Uploaded");
                 await Navigation.PopPopupAsync();
             }
         }*/
        /*
        public async void UploadImage()
        {
            var connection = await apiService.CheckConnection();
            var accesstoken = Settings.AccessToken;

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.GetStream()),
                "\"file\"",
                $"\"{file.Path}\"");

            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
            }

            var imageExercice = new AddImageExercice
            {
                imageFile = content,
                idExercise = Exercice.id
            };

            var response = await apiService.Save<AddImageExercice>(
                 "http://phoneofficine.it",
                 "/niini-gim",
                 "/pictureExercise/save",
                 accesstoken,
                 imageExercice);

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }

            DependencyService.Get<INotification>().CreateNotification("GYM TECH", "Image Uploaded");
            await Navigation.PopPopupAsync();
        }
        */
        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                "Where do you take the picture?",
                "Cancel",
                null,
                "From Gallery",
                "From Camera");

            if (source == "Cancel")
            {
                this.file = null;
                return;
            }

            if (source == "From Camera")
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Pictures",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
        }
        #endregion

        #region Commands
        public ICommand ChangeImageCommand => new RelayCommand(this.ChangeImage);
        public ICommand SaveBranch
        {
            get
            {
                return new Command(() =>
                {
                    // UploadImage();
                    AddNewBranch();
                });
            }
        }
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    Navigation.PopPopupAsync();
                });
            }
        }
        #endregion
    }
}

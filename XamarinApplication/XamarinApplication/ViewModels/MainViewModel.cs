
namespace XamarinApplication.ViewModels
{
    using System.Collections.Generic;
    using Models;

    public class MainViewModel
    {
        #region Properties
        public TokenResponse Token
        {
            get;
            set;
        }
        //string accesstoken = Settings.AccessToken;
        #endregion

        #region ViewModels
        public LoginViewModel Login
        {
            get;
            set;
        }
        #endregion
        #region Constructors
        public MainViewModel()
        {
            instance = this;
            //this.Login = new LoginViewModel();
        }
        #endregion

        #region Singleton
        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }
        #endregion
    }
}

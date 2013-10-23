Steps to run the Demo Application
- Create a Windows Azure Mobile Service
- Search in Project for //TODO: Add specifict mobile service information
- Add mobile service uri and app key

        //TODO: Add specific mobile service information
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://<<add-mobile-service-name-here>>.azure-mobile.net/",
            "<<add-mobile-service-app-key-here>>"
            #region 05_01 Request Interception
            //,new RequestInterceptor()
            #endregion 05_01 Request Interception
        );
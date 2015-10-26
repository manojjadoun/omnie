﻿namespace VimeoDotNet.Net
{
    public class ApiRequestFactory : IApiRequestFactory
    {
        public IApiRequest GetApiRequest()
        {
            return new ApiRequest();
        }

        public IApiRequest GetApiRequest(string clientId, string clientSecret)
        {
            return new ApiRequest(clientId, clientSecret);
        }

        public IApiRequest GetApiRequest(string accessToken)
        {
            return new ApiRequest(accessToken);
        }
    }
}
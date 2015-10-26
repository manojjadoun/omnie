﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Contrib;
using VimeoDotNet.Constants;
using VimeoDotNet.Models;
using VimeoDotNet.Net;

namespace VimeoDotNet.Authorization
{
    public class AuthorizationClient : IAuthorizationClient
    {
        #region Private Properties

        protected string ClientId { get; set; }
        protected string ClientSecret { get; set; }

        #endregion

        #region Constructors

        public AuthorizationClient(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string GetAuthorizationEndpoint(string redirectUri, IEnumerable<string> scope, string state)
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new InvalidOperationException("Authorization.ClientId should be a non-null, non-whitespace string");
            }
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                throw new ArgumentException("redirectUri should be a valid Uri");
            }

            string queryString = BuildAuthorizeQueryString(redirectUri, scope, state);
            return BuildUrl(Endpoints.Authorize, queryString);
        }

        public AccessTokenResponse GetAccessToken(string authorizationCode, string redirectUri)
        {
            return GetAccessTokenAsync(authorizationCode, redirectUri).Result;
        }

        public async Task<AccessTokenResponse> GetAccessTokenAsync(string authorizationCode, string redirectUri)
        {
            ApiRequest request = BuildAccessTokenRequest(authorizationCode, redirectUri);
            IRestResponse<AccessTokenResponse> result = await request.ExecuteRequestAsync<AccessTokenResponse>();
            return result.Data;
        }

        #endregion

        #region Private Methods

        protected ApiRequest BuildAccessTokenRequest(string authorizationCode, string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new InvalidOperationException("Authorization.ClientId should be a non-null, non-whitespace string");
            }
            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new InvalidOperationException(
                    "Authorization.ClientSecret should be a non-null, non-whitespace string");
            }
            if (string.IsNullOrWhiteSpace(authorizationCode))
            {
                throw new ArgumentException("authorizationCode should be a non-null, non-whitespace string");
            }
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                throw new ArgumentException("redirectUri should be a valid Uri");
            }

            var request = new ApiRequest(ClientId, ClientSecret);
            request.Method = Method.POST;
            request.Path = Endpoints.AccessToken;
            SetAccessTokenQueryParams(request, authorizationCode, redirectUri);

            return request;
        }

        protected string BuildUrl(string route, string queryString)
        {
            return string.Format("{0}://{1}{2}{3}", Request.DefaultProtocol, Request.DefaultHostName, route, queryString);
        }

        protected void SetAccessTokenQueryParams(ApiRequest request, string authorizationCode, string redirectUri)
        {
            request.Query.Add("grant_type", "authorization_code");
            request.Query.Add("code", authorizationCode);
            request.Query.Add("redirect_uri", redirectUri);
        }

        protected string BuildAuthorizeQueryString(string redirectUri, IEnumerable<string> scope, string state)
        {
            var qsParams = new Dictionary<string, string>
            {
                {"response_type", "code"},
                {"client_id", ClientId},
                {"redirect_uri", redirectUri}
            };

            if (scope == null)
            {
                qsParams.Add("scope", Scopes.Public);
            }
            else
            {
                qsParams.Add("scope", string.Join(" ", scope.ToArray()));
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                qsParams.Add("state", state);
            }

            return GetQueryString(qsParams);
        }

        protected string GetQueryString(IDictionary<string, string> queryParams)
        {
            var sb = new StringBuilder("");
            foreach (var qsParam in queryParams)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(qsParam.Key), HttpUtility.UrlEncode(qsParam.Value));
            }
            sb.Insert(0, "?");
            return sb.ToString();
        }

        #endregion
    }
}
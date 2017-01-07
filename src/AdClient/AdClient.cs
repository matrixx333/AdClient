using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using AdClient.Models;
using System.Net.Mail;
using AdClient.Models.Requests;

namespace AdClient
{
    public class AdClient
    {
        private HttpClient _usersClient;
        private HttpClient _groupsClient;
        private string _usersUri;
        private string _groupsUri;

        public AdClient(string baseAddress)
        {
            _usersUri = $"{baseAddress}/api/v1/users/";
            _groupsUri = $"{baseAddress}/api/v1/groups/";

            _usersClient = new HttpClient();
            _usersClient.BaseAddress = new Uri(_usersUri);
            _usersClient.DefaultRequestHeaders.Accept.Clear();
            _usersClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _groupsClient = new HttpClient();
            _groupsClient.BaseAddress = new Uri(_groupsUri);
            _groupsClient.DefaultRequestHeaders.Accept.Clear();
            _groupsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region GROUPS

        // missing RemoveAllUserGroups(string samAccountName)

        public Group GetGroup(string groupName)
        {
            if (groupName == null) return null;

            var response = _groupsClient.GetAsync($"{_groupsUri}/{groupName}").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<Group>().Result;
        }

        public IEnumerable<Group> GetAllGroups()
        {
            var response = _groupsClient.GetAsync($"{_groupsUri}").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<IEnumerable<Group>>().Result;
        }

        public bool RemoveUserFromGroup(string samAccountName, string groupName)
        {
            var wasSuccessful = false;

            if (samAccountName == null) return wasSuccessful;
            if (groupName == null) return wasSuccessful;

            var request = new ChangeGroupRequest()
            {
                GroupName = groupName,
                SamAccountName = samAccountName
            };

            var response = _groupsClient.PostAsJsonAsync($"{_groupsUri}/remove-from-group/", request).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<bool>().Result;
        }

        public bool AddUserToGroup(string samAccountName, string groupName)
        {
            var wasSuccessful = false;

            if (samAccountName == null) return wasSuccessful;
            if (groupName == null) return wasSuccessful;

            var request = new ChangeGroupRequest()
            {
                GroupName = groupName,
                SamAccountName = samAccountName
            };

            var response = _groupsClient.PostAsJsonAsync($"{_groupsUri}/add-to-group/", request).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<bool>().Result;
        }

        #endregion

        #region USERS

        public bool ValidateCredentials(string userName, string password)
        {
            var wasSuccessful = false;

            if (userName == null) return wasSuccessful;
            if (password == null) return wasSuccessful;

            var creds = new Credentials()
            {
                Password = password,
                Username = userName
            };

            var response = _usersClient.PostAsJsonAsync($"{_usersUri}/validate/", creds).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<bool>().Result;
        }

        public IEnumerable<Group> GetGroups(string samAccountName)
        {
            if (samAccountName == null) return null;

            var response = _usersClient.GetAsync($"{_usersUri}/{samAccountName}/groups").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<IEnumerable<Group>>().Result;
        }


        public bool IsUserGroupMember(string samAccountName, string groupName)
        {
            var wasSuccessful = false;

            if (samAccountName == null) return wasSuccessful;
            if (groupName == null) return wasSuccessful;

            var request = new IsGroupMemberRequest() { GroupName = groupName };

            var response = _usersClient.PostAsJsonAsync($"{_usersUri}/{samAccountName}/is-member", request).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<bool>().Result;
        }

        public User GetUser(string samAccountName)
        {
            if (samAccountName == null) return null;

            var response = _usersClient.GetAsync($"{_usersUri}/{samAccountName}").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<User>().Result;
        }

        public bool MoveUser(string userDistinguishedName, string newContainer)
        {
            var wasSuccessful = false;

            if (userDistinguishedName == null) return wasSuccessful;
            if (newContainer == null) return wasSuccessful;

            var request = new MoveUserRequest()
            {
                NewContainer = newContainer,
                UserDistinguishedName = userDistinguishedName
            };

            var response = _usersClient.PostAsJsonAsync($"{_usersUri}/move", request).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<bool>().Result;
        }

        public User GetUserByEmail(string emailAddress)
        {
            if (emailAddress == null) return null;

            var address = new MailAddress(emailAddress);

            var response = _usersClient.GetAsync($"{_usersUri}/{address.User}").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<User>().Result;
        }

        public Guid? GetGuid(string samAccountName)
        {
            if (samAccountName == null) return Guid.Empty;

            var response = _usersClient.GetAsync($"{_usersUri}/{samAccountName}").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<User>().Result.Guid;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var response = _usersClient.GetAsync($"{_usersUri}").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<IEnumerable<User>>().Result;
        }

        public bool Update(string samAccountName, UpdateUserRequest request)
        {
            var wasSuccessful = false;

            if (samAccountName == null) return wasSuccessful;
            if (request == null) return wasSuccessful;

            var response = _usersClient.PutAsJsonAsync($"{_usersUri}/{samAccountName}", request).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<bool>().Result;
        }

        #endregion
    }
}


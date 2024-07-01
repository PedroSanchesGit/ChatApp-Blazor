using System.DirectoryServices;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class ActiveDirectoryService
    {

        #region Properties
        public IConfiguration Configuration { get; set; }

        #endregion

        #region Constructor
        public ActiveDirectoryService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method that gets the value which represents the connection string for LDAP
        /// </summary>
        /// <returns>connection string</returns>
        public string GetCurrentDomainPath()
        {
            return Configuration["PathLDAP"];
        }

        /// <summary>
        /// Method that finds a user that is part of the active directory
        /// </summary>
        /// <param name="user">Username that will be used as a filter to retrieve the information about a specific user</param>
        /// <param name="info">Key that allows getting a specific value related to the user</param>
        /// <returns>The Search Result</returns>
        public async Task<SearchResult> FindUserSearchResult(string user, string[] infos)
        {
            try
            {
                var username = user.Split("\\");


                using (DirectoryEntry entry = new DirectoryEntry(GetCurrentDomainPath()))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        SearchResultCollection searchAll = searcher.FindAll();
                        searcher.Filter = $"(samaccountname={username[1]})";
                        foreach (var info in infos)
                        {
                            searcher.PropertiesToLoad.Add(info);
                        }
                        return searcher.FindOne();
                    }
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        //employeetype;samaccountname;thumbnailphoto;displayName
        /// <summary>
        /// Method that retrieves the information needed from a specific user
        /// </summary>
        /// <param name="user">Username that will be used as a filter to retrieve the information about a specific user</param>
        /// <param name="info">Key that allows getting a specific value related to the user</param>
        /// <returns></returns>
        public async Task<string> GetInfoByUserLoggedIn(string user, string info)
        {
            SearchResult searchByUser = await FindUserSearchResult(user, new string[] { info });

            if (searchByUser != null)
            {
                ResultPropertyCollection fields = searchByUser.Properties;

                foreach (String ldapField in fields.PropertyNames)
                {
                    foreach (var myCollection in fields[ldapField])
                    {
                        if (String.Equals(ldapField, info))
                        {
                            return myCollection.ToString();
                        }

                    }
                }

            }

            return null;
        }

        /// <summary>
        /// Method that catches and treats all the OU roles from a specifc AD User
        /// </summary>
        /// <param name="user">user from the ad</param>
        /// <returns>list of string values identifying all the user roles</returns>
        public List<string> ListOuFromUser(string user)
        {

            List<string> ous = new();
            try
            {
                SearchResult searchResult = FindUserSearchResult(user, new string[] { "distinguishedName" }).Result;

                string resultString = searchResult.Path;

                string[] explodeString = resultString.Split(",");

                foreach (var item in explodeString)
                {
                    if (item.StartsWith("OU="))
                    {
                        ous.Add(item.Split("OU=")[1]);
                    }
                }

                return ous;
            }
            catch (Exception)
            {

                return ous;
            }


        }

        #endregion


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Collections;
using System.DirectoryServices;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Data.SqlClient;

namespace WUFT.NET.App_Start
{
    public class clsLDAP_Authentication
    {
        private string _path;
        private string _filterAttribute;

        public clsLDAP_Authentication(string path)
        {
            _path = path;
        }

        public clsLDAP_Authentication()
        {
        }

        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(&(SAMAccountName=" + username + ")(|(employeeBadgeType=GB)(employeeBadgeType=BB)))";// +_filterAttribute + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }

            return true;
        }
        public string GetGroups()
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(&(cn=" + _filterAttribute + ")(|(employeeBadgeType=GB)(employeeBadgeType=BB)))";// +_filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();

            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return groupNames.ToString();
        }
        public void GetUserDetails(string username, out string Name, out string Email, out string WWID)
        {
            Name = "";
            Email = "";
            WWID = "";
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "amr");
                PrincipalContext ctx1 = new PrincipalContext(ContextType.Domain, "ccr");
                PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain, "gar");
                PrincipalContext ctx3 = new PrincipalContext(ContextType.Domain, "ger");
                //ctx.ConnectedServer = "bgsgar202.gar.corp.intel.com";
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx1, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx2, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx3, IdentityType.SamAccountName, username);

                Name = user.DisplayName;
                Email = user.EmailAddress;
                WWID = user.EmployeeId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //return email;
        }
        public string GetWWId(string username)
        {
            string WWID = "";
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "amr");
                PrincipalContext ctx1 = new PrincipalContext(ContextType.Domain, "ccr");
                PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain, "gar");
                PrincipalContext ctx3 = new PrincipalContext(ContextType.Domain, "ger");
                //ctx.ConnectedServer = "bgsgar202.gar.corp.intel.com";
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx1, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx2, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx3, IdentityType.SamAccountName, username);
                WWID = user.EmployeeId;
            }
            catch (Exception ex)
            { }
            return WWID;
        }
        public string GetUserName(string username)
        {
            string UserName = "";
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "amr");
                PrincipalContext ctx1 = new PrincipalContext(ContextType.Domain, "ccr");
                PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain, "gar");
                PrincipalContext ctx3 = new PrincipalContext(ContextType.Domain, "ger");
                //ctx.ConnectedServer = "bgsgar202.gar.corp.intel.com";
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx1, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx2, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx3, IdentityType.SamAccountName, username);
                UserName = user.DisplayName;
            }
            catch (Exception ex)
            { }
            return UserName;
        }
        public UserPrincipal GetManager1(UserPrincipal user)
        {
            UserPrincipal result = null;

            if (user != null)
            {
                // get the DirectoryEntry behind the UserPrincipal object
                DirectoryEntry dirEntryForUser = user.GetUnderlyingObject() as DirectoryEntry;

                if (dirEntryForUser != null)
                {
                    // check to see if we have a manager name - if so, grab it
                    if (dirEntryForUser.Properties["manager"] != null)
                    {
                        string managerDN = dirEntryForUser.Properties["manager"][0].ToString().Trim();
                        string[] tokens = managerDN.Split(',');
                        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "amr");
                        PrincipalContext ctx1 = new PrincipalContext(ContextType.Domain, "ccr");
                        PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain, "gar");
                        PrincipalContext ctx3 = new PrincipalContext(ContextType.Domain, "ger");

                        // find the manager UserPrincipal via the managerDN 
                        result = UserPrincipal.FindByIdentity(ctx, managerDN);
                        if (result == null)
                            result = UserPrincipal.FindByIdentity(ctx1, managerDN);
                        if (result == null)
                            result = UserPrincipal.FindByIdentity(ctx2, managerDN);
                        if (result == null)
                            result = UserPrincipal.FindByIdentity(ctx3, managerDN);
                    }
                }
            }

            return result;
        }
        public string GetManager(string username)
        {
            string Manager = "";
            try
            {

                UserPrincipal myManager;
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "amr");
                PrincipalContext ctx1 = new PrincipalContext(ContextType.Domain, "ccr");
                PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain, "gar");
                PrincipalContext ctx3 = new PrincipalContext(ContextType.Domain, "ger");
                //ctx.ConnectedServer = "bgsgar202.gar.corp.intel.com";
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx1, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx2, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx3, IdentityType.SamAccountName, username);
                myManager = GetManager1(user);

                // get the manager for myself                 
                Manager = myManager.EmailAddress;
                //string wwid = myManager.EmployeeId;
            }
            catch (Exception ex)
            { }
            return Manager;
        }
        public string GetManagerEmailId(string username)
        {
            string email = "";
            try
            {
                username = GetManager(username);
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "amr");
                PrincipalContext ctx1 = new PrincipalContext(ContextType.Domain, "ccr");
                PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain, "gar");
                PrincipalContext ctx3 = new PrincipalContext(ContextType.Domain, "ger");
                //ctx.ConnectedServer = "bgsgar202.gar.corp.intel.com";
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx1, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx2, IdentityType.SamAccountName, username);
                if (user == null)
                    user = UserPrincipal.FindByIdentity(ctx3, IdentityType.SamAccountName, username);

                email = user.EmailAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return email;
        }

        public void GetUserNameByWWId(string WWID, out string Name, out string Email, out string username, out string wwid)
        {
            Name = "";
            Email = "";
            username = "";
            wwid = "";
            try
            {
                DirectoryEntry entryGAR = new DirectoryEntry("LDAP://gar.corp.intel.com");
                DirectoryEntry entryGER = new DirectoryEntry("LDAP://ger.corp.intel.com");
                DirectoryEntry entryCCR = new DirectoryEntry("LDAP://ccr.corp.intel.com");
                DirectoryEntry entryAMR = new DirectoryEntry("LDAP://amr.corp.intel.com");

                entryGAR.Username = "grp_bitindia";
                entryGAR.Password = "xseos98@";
                entryGER.Username = "grp_bitindia";
                entryGER.Password = "xseos98@";
                entryCCR.Username = "grp_bitindia";
                entryCCR.Password = "xseos98@";
                entryAMR.Username = "grp_bitindia";
                entryAMR.Password = "xseos98@";
                DirectorySearcher search;
                SearchResult result;

                search = new DirectorySearcher(entryGAR);
                search.Filter = "(&(EmployeeId=" + WWID + ")(|(employeeBadgeType=GB)(employeeBadgeType=BB)))";// +_filterAttribute + ")";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("EmployeeId");
                search.PropertiesToLoad.Add("samaccountname");

                result = search.FindOne();

                if (result != null)
                {
                    username = result.Properties["samaccountname"][0].ToString();
                    Email = result.Properties["mail"][0].ToString();
                    Name = result.Properties["cn"][0].ToString();
                    wwid = result.Properties["EmployeeId"][0].ToString();
                }
                else
                {
                    search = new DirectorySearcher(entryGER);
                    search.Filter = "(&(EmployeeId=" + WWID + ")(|(employeeBadgeType=GB)(employeeBadgeType=BB)))";// +_filterAttribute + ")";
                    search.PropertiesToLoad.Add("cn");
                    search.PropertiesToLoad.Add("mail");
                    search.PropertiesToLoad.Add("EmployeeId");
                    search.PropertiesToLoad.Add("samaccountname");
                    result = search.FindOne();
                    if (result != null)
                    {
                        username = result.Properties["samaccountname"][0].ToString();
                        Email = result.Properties["mail"][0].ToString();
                        Name = result.Properties["cn"][0].ToString();
                        wwid = result.Properties["EmployeeId"][0].ToString();
                    }
                    else
                    {
                        search = new DirectorySearcher(entryCCR);
                        search.Filter = "(&(EmployeeId=" + WWID + ")(|(employeeBadgeType=GB)(employeeBadgeType=BB)))";// +_filterAttribute + ")";
                        search.PropertiesToLoad.Add("cn");
                        search.PropertiesToLoad.Add("mail");
                        search.PropertiesToLoad.Add("EmployeeId");
                        search.PropertiesToLoad.Add("samaccountname");
                        result = search.FindOne();
                        if (result != null)
                        {
                            username = result.Properties["samaccountname"][0].ToString();
                            Email = result.Properties["mail"][0].ToString();
                            Name = result.Properties["cn"][0].ToString();
                            wwid = result.Properties["EmployeeId"][0].ToString();
                        }
                        else
                        {
                            search = new DirectorySearcher(entryAMR);
                            search.Filter = "(&(EmployeeId=" + WWID + ")(|(employeeBadgeType=GB)(employeeBadgeType=BB)))";// +_filterAttribute + ")";
                            search.PropertiesToLoad.Add("cn");
                            search.PropertiesToLoad.Add("mail");
                            search.PropertiesToLoad.Add("EmployeeId");
                            search.PropertiesToLoad.Add("samaccountname");
                            result = search.FindOne();
                            if (result != null)
                            {
                                username = result.Properties["samaccountname"][0].ToString();
                                Email = result.Properties["mail"][0].ToString();
                                Name = result.Properties["cn"][0].ToString();
                                wwid = result.Properties["EmployeeId"][0].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public void GetUserNameByEmailID(string EmailID, out string Name, out string Email, out string username, out string wwid)
        {
            Name = "";
            Email = "";
            username = "";
            wwid = "";
            try
            {
                DirectoryEntry entryGAR = new DirectoryEntry("LDAP://gar.corp.intel.com");
                DirectoryEntry entryGER = new DirectoryEntry("LDAP://ger.corp.intel.com");
                DirectoryEntry entryCCR = new DirectoryEntry("LDAP://ccr.corp.intel.com");
                DirectoryEntry entryAMR = new DirectoryEntry("LDAP://amr.corp.intel.com");

                entryGAR.Username = "grp_bitindia";
                entryGAR.Password = "xseos98@";
                entryGER.Username = "grp_bitindia";
                entryGER.Password = "xseos98@";
                entryCCR.Username = "grp_bitindia";
                entryCCR.Password = "xseos98@";
                entryAMR.Username = "grp_bitindia";
                entryAMR.Password = "xseos98@";
                DirectorySearcher search;
                SearchResult result;

                search = new DirectorySearcher(entryGAR);
                search.Filter = "(mail=" + EmailID + "*)";// +_filterAttribute + ")";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("EmployeeId");
                search.PropertiesToLoad.Add("samaccountname");

                result = search.FindOne();

                if (result != null)
                {
                    username = result.Properties["samaccountname"][0].ToString();
                    Email = result.Properties["mail"][0].ToString();
                    Name = result.Properties["cn"][0].ToString();
                    wwid = result.Properties["EmployeeId"][0].ToString();
                }
                else
                {
                    search = new DirectorySearcher(entryGER);
                    search.Filter = "(mail=" + EmailID + "*)";// +_filterAttribute + ")";
                    search.PropertiesToLoad.Add("cn");
                    search.PropertiesToLoad.Add("mail");
                    search.PropertiesToLoad.Add("EmployeeId");
                    search.PropertiesToLoad.Add("samaccountname");
                    result = search.FindOne();
                    if (result != null)
                    {
                        username = result.Properties["samaccountname"][0].ToString();
                        Email = result.Properties["mail"][0].ToString();
                        Name = result.Properties["cn"][0].ToString();
                        wwid = result.Properties["EmployeeId"][0].ToString();
                    }
                    else
                    {
                        search = new DirectorySearcher(entryCCR);
                        search.Filter = "(mail=" + EmailID + "*)";// +_filterAttribute + ")";
                        search.PropertiesToLoad.Add("cn");
                        search.PropertiesToLoad.Add("mail");
                        search.PropertiesToLoad.Add("EmployeeId");
                        search.PropertiesToLoad.Add("samaccountname");
                        result = search.FindOne();
                        if (result != null)
                        {
                            username = result.Properties["samaccountname"][0].ToString();
                            Email = result.Properties["mail"][0].ToString();
                            Name = result.Properties["cn"][0].ToString();
                            wwid = result.Properties["EmployeeId"][0].ToString();
                        }
                        else
                        {
                            search = new DirectorySearcher(entryAMR);
                            search.Filter = "(mail=" + EmailID + "*)";// +_filterAttribute + ")";
                            search.PropertiesToLoad.Add("cn");
                            search.PropertiesToLoad.Add("mail");
                            search.PropertiesToLoad.Add("EmployeeId");
                            search.PropertiesToLoad.Add("samaccountname");
                            result = search.FindOne();
                            if (result != null)
                            {
                                username = result.Properties["samaccountname"][0].ToString();
                                Email = result.Properties["mail"][0].ToString();
                                Name = result.Properties["cn"][0].ToString();
                                wwid = result.Properties["EmployeeId"][0].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public void GetUserNameByIDSID(string IDSID, out string Name, out string Email, out string username, out string wwid)
        {
            Name = "";
            Email = "";
            username = "";
            wwid = "";
            try
            {
                DirectoryEntry entryGAR = new DirectoryEntry("LDAP://gar.corp.intel.com");
                DirectoryEntry entryGER = new DirectoryEntry("LDAP://ger.corp.intel.com");
                DirectoryEntry entryCCR = new DirectoryEntry("LDAP://ccr.corp.intel.com");
                DirectoryEntry entryAMR = new DirectoryEntry("LDAP://amr.corp.intel.com");

                entryGAR.Username = "grp_bitindia";
                entryGAR.Password = "xseos98@";
                entryGER.Username = "grp_bitindia";
                entryGER.Password = "xseos98@";
                entryCCR.Username = "grp_bitindia";
                entryCCR.Password = "xseos98@";
                entryAMR.Username = "grp_bitindia";
                entryAMR.Password = "xseos98@";
                DirectorySearcher search;
                SearchResult result;

                search = new DirectorySearcher(entryGAR);
                search.Filter = "(samaccountname=" + IDSID + "*)";// +_filterAttribute + ")";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("EmployeeId");
                search.PropertiesToLoad.Add("samaccountname");

                result = search.FindOne();

                if (result != null)
                {
                    username = result.Properties["samaccountname"][0].ToString();
                    Email = result.Properties["mail"][0].ToString();                    
                    Name = result.Properties["cn"][0].ToString();
                    wwid = result.Properties["EmployeeId"][0].ToString();
                    if (Name.EndsWith("X"))
                    Name = Name.Substring(0, Name.Length - 1);

                }
                else
                {
                    search = new DirectorySearcher(entryGER);
                     search.Filter = "(samaccountname=" + IDSID + "*)";// +_filterAttribute + ")";
                    search.PropertiesToLoad.Add("cn");
                    search.PropertiesToLoad.Add("mail");
                    search.PropertiesToLoad.Add("EmployeeId");
                    search.PropertiesToLoad.Add("samaccountname");
                    result = search.FindOne();
                    if (result != null)
                    {
                        username = result.Properties["samaccountname"][0].ToString();
                        Email = result.Properties["mail"][0].ToString();
                        Name = result.Properties["cn"][0].ToString();
                        wwid = result.Properties["EmployeeId"][0].ToString();
                        if (Name.EndsWith("X"))
                            Name = Name.Substring(0, Name.Length - 1);
                    }
                    else
                    {
                        search = new DirectorySearcher(entryCCR);
                         search.Filter = "(samaccountname=" + IDSID + "*)";// +_filterAttribute + ")";
                        search.PropertiesToLoad.Add("cn");
                        search.PropertiesToLoad.Add("mail");
                        search.PropertiesToLoad.Add("EmployeeId");
                        search.PropertiesToLoad.Add("samaccountname");
                        result = search.FindOne();
                        if (result != null)
                        {
                            username = result.Properties["samaccountname"][0].ToString();
                            Email = result.Properties["mail"][0].ToString();
                            Name = result.Properties["cn"][0].ToString();
                            wwid = result.Properties["EmployeeId"][0].ToString();
                            if (Name.EndsWith("X"))
                                Name = Name.Substring(0, Name.Length - 1);
                        }
                        else
                        {
                            search = new DirectorySearcher(entryAMR);
                             search.Filter = "(samaccountname=" + IDSID + "*)";// +_filterAttribute + ")";
                            search.PropertiesToLoad.Add("cn");
                            search.PropertiesToLoad.Add("mail");
                            search.PropertiesToLoad.Add("EmployeeId");
                            search.PropertiesToLoad.Add("samaccountname");
                            result = search.FindOne();
                            if (result != null)
                            {
                                username = result.Properties["samaccountname"][0].ToString();
                                Email = result.Properties["mail"][0].ToString();
                                Name = result.Properties["cn"][0].ToString();
                                wwid = result.Properties["EmployeeId"][0].ToString();
                                if (Name.EndsWith("X"))
                                    Name = Name.Substring(0, Name.Length - 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }


    }
}
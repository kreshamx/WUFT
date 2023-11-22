using System.Linq;
using System.Security.Principal;
using WUFT.DATA;
using WUFT.MODEL;
using System.Xml;
using WUFT.NET.App_Start;

namespace WUFT.NET.Util
{
    public class ISEPrincipal : IPrincipal
    {
        private WUFT.NET.WorkerService.WorkerService _workerService = new WUFT.NET.WorkerService.WorkerService();
        clsLDAP_Authentication objAuth = new clsLDAP_Authentication();
        public ISEPrincipal(IIdentity identity, IUnitOfWork uow)
        {
            string idsid = identity.Name.Split('\\')[1].ToLower();
            Identity = identity;
            WUFTUser = uow.Users.GetAll().SingleOrDefault(x => x.IdSid == idsid);
            if(WUFTUser == null)
            {
                var _newPerson = AddPerson(idsid);
                if(_newPerson != null)
                {
                    uow.Users.Add(_newPerson);
                    uow.SaveChangesAsync();
                    WUFTUser = _newPerson;
                }
            }
            
        }

        public WUFTUser WUFTUser { get; set; }
        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            return System.Web.Security.Roles.IsUserInRole(Identity.Name, role);
        }

        public bool IsQRE()
        {
            return IsInRole(@"AMR\ISE Logistics_WUFT_QRE");
        }

        public bool IsOperator()
        {
            return IsInRole(@"AMR\ISE Logistics_WUFT_Operator");
        }

        public bool IsAdmin()
        {
            return IsInRole(@"AMR\ISE Logistics_WUFT_Admin");
        }

        public bool IsDeveloper()
        {
            return IsInRole(@"AMR\ISE Logistics_ISE_Developer") || IsInRole(@"GAR\FlexAppsSupport");
        }
        

        public WUFTUser AddPerson(string idsid)
        {
            WUFTUser _newPerson = new WUFTUser();
            if (!string.IsNullOrEmpty(idsid)) // Underscore denotes a faceless account which won't be found in CDIS
            {
                string IDSID = "";
                string Name = "";
                string Email = "";
                string wwid = "";
                objAuth.GetUserNameByIDSID(idsid, out Name, out Email, out IDSID, out wwid);
                if (wwid != "")
                {
                    _newPerson.FullName = Name.ToUpper();
                    _newPerson.EmailAddress = Email;
                    _newPerson.WWID = wwid;
                    _newPerson.IdSid = idsid;
                }

                ////Get info from CDIS
                //XmlNode rootNode;
                //rootNode = _workerService.GetWorkerDetailByIDSID("ISEA", "ISEA4u2c", idsid);
                //XmlNode workerNode = rootNode.ChildNodes[0];

                //if (workerNode != null)
                //{
                //    _newPerson.FullName = workerNode.SelectSingleNode("BookName").InnerText;
                //    _newPerson.EmailAddress = workerNode.SelectSingleNode("DomainAddress").InnerText;
                //    _newPerson.WWID = workerNode.SelectSingleNode("WWID").InnerText;
                //    _newPerson.IdSid = idsid;
                //}
            }
            return _newPerson;
        }


        
    }
}
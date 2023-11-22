using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WUFT.DATA;
using WUFT.NET.ViewModels.Shared;

namespace WUFT.NET.Util
{
    public class PersonHelper
    {
        private readonly IUnitOfWork uow;
        public PersonHelper(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public PersonModel GetPersonModelByIdsid(string idsid)
        {
            var _user = uow.Users.GetAll().FirstOrDefault(x=>x.IdSid == idsid);
            if(_user == null)
                return new PersonModel{IdSid = idsid};
            else
            {
                return new PersonModel{
                    IdSid = idsid,
                    FullName = _user.FullName,
                    Email = _user.EmailAddress,
                    WWID = _user.WWID,
                    PhoneBookName = _user.FullName.Split(',')[1] + _user.FullName.Split(',')[0]
                };
                
            }           
        }
    }
}
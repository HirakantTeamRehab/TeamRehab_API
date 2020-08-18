using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.DataModel;
using Team.Rehab.Repository.UnitOfwork;
using Team.Rehab.BusinessEntities;
using AutoMapper;

namespace Team.Rehab.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IUnitOfwork _unitOfWork;
        private readonly IProcedureManagement _procedureManagement;



        public UserRepository()
        {
            this._unitOfWork = new UnitOfWork();
            this._procedureManagement = new ProcedureManagement();
        }

       // private IUnitOfwork _unitOfWork;

        /// <summary>
        /// Get an user based on email address
        /// </summary>
        /// <param name="EmailAddress"></param>
        /// <returns>User object</returns>
        //public List<tblUser> GetUser()
        //{

        //    //User _protectedResource = new User();
        //    ////Check users access(autherization) define in XML.                
        //    //_protectedResource.Country_cISOCode = ApplicationIdentity.Country;
        //    //_protectedResource.CreatedBy = ApplicationIdentity.Uid;

        //    //if (!AuthorizationManager.CheckAccess(ApplicationIdentity, _protectedResource, Operations.View, Modules.User))
        //    //{
        //    //    throw new UnauthorizedAccessException();
        //    //}

        //    List<tblUser> user = _unitOfWork.UserEntityRepo.Get().ToList();
        //    if (user.Count() == 1)
        //    {
        //        return user;
        //    }
        //    else
        //    {
        //        throw new UnauthorizedAccessException();
        //    }
        //}
        public List<tblAppUser> GetUser(string UserName, string Password)
        {
            List<tblAppUser> user = new List<tblAppUser>();
            //List<SP_ValidateUser_Result> response = new List<SP_ValidateUser_Result>();
          

           // var response = _procedureManagement.GetUser(UserName,Password);
              user = _unitOfWork.AppUserRepo.Get(c => c.EmailId.Equals(UserName) && c.PhoneNumber.Equals(Password)).ToList();
            if (user.Count() >0)
            {
                return user;
            }
            else
            {
                return user;
            }
        }

        public List<ValidateUserEntity> AuthenticateUser(string UserName, string Password)
        {
            List<ValidateUserEntity> postValidateEntity = new List<ValidateUserEntity>();
            List<SP_ValidateUser_Result> preValidateEntity = new List<SP_ValidateUser_Result>();
            //List<SP_ValidateUser_Result> response = new List<SP_ValidateUser_Result>();
            preValidateEntity = _procedureManagement.GetUser(UserName, Password).ToList();

            //===============Mapper===========================================
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<tblRefreshTokens, RefreshToken>();
            });

            IMapper mapper = config.CreateMapper();
            postValidateEntity = mapper.Map<List<SP_ValidateUser_Result>, List<ValidateUserEntity>>(preValidateEntity);
            //===============mapper end==========================================
           
                return postValidateEntity;
           
        }
    }
   
}

using System.Linq;
using System.Transactions;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.DataModel;
using Team.Rehab.Repository.UnitOfwork;
using Team.Rehab.BusinessEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Data.Entity;

using System.IO;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using AutoMapper;

namespace Team.Rehab.Repository.Repository
{
   public class AppUsersRepository : IAppUsers
    {
        private readonly IUnitOfwork _unitOfWork;

        private RehabEntities _ctx;
        public AppUsersRepository()
        {
            this._unitOfWork = new UnitOfWork();
            _ctx = new RehabEntities();
        }


        /// <summary>
        /// Fetches product details by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public AppUsersEntity GetAppUserById(int AUrowid)
        {
            AppUsersEntity appUser = new AppUsersEntity();
            var users = _unitOfWork.AppUserRepo.GetByID(AUrowid);
            if (users != null)
            {
                //===============Mapper===========================================
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<tblAppUser, AppUsersEntity>();
                });

                IMapper mapper = config.CreateMapper();
                appUser = mapper.Map<tblAppUser, AppUsersEntity>(users);
                //===============mapper end==========================================

               
                return appUser;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the products.
        /// </summary>
        /// <returns></returns>
        public List<AppUsersEntity> GetAllAppUsers()
        {
            List<AppUsersEntity> appUsers = new List<AppUsersEntity>(); 
            var users = _unitOfWork.AppUserRepo.Get().ToList();
            if (users.Any())
            {
                //===============Mapper===========================================
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<tblAppUser, AppUsersEntity>();
                });

                IMapper mapper = config.CreateMapper();
                appUsers = mapper.Map<List<tblAppUser>, List<AppUsersEntity>>(users);
                //===============mapper end==========================================
                return appUsers;
            }
            return null;
        }

        /// <summary>
        /// Creates a product
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public int CreateAppUser(AppUsersEntity appUserEntity)
        {
            using (var scope = new TransactionScope())
            {
                var AppUsers = new tblAppUser
                {
                    AUrowid = appUserEntity.AUrowid,
                    FirstName = appUserEntity.FirstName,
                    LastName = appUserEntity.LastName,
                    PhoneNumber = appUserEntity.PhoneNumber,
                    EmailId = appUserEntity.EmailId,
                    NPINumber = appUserEntity.NPINumber,
                    Access = appUserEntity.Access,
                   // createdby = appUserEntity.createdby,
                    createdby = "Admin",
                    createdts = DateTime.Now,
                    updatedby = "Admin",
                    updatedts = DateTime.Now
                };
                _unitOfWork.AppUserRepo.Insert(AppUsers);
                _unitOfWork.Save();
                scope.Complete();
                return AppUsers.AUrowid;
            }
        }

        /// <summary>
        /// Updates a product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public bool UpdateAppUser(AppUsersEntity appUserEntity)
        {
            var success = false;
            if (appUserEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var user = _unitOfWork.AppUserRepo.GetByID(appUserEntity.AUrowid);
                    if (user != null)
                    {
                        //user.ProductName = productEntity.ProductName;
                        user.AUrowid = appUserEntity.AUrowid;
                        user.FirstName = appUserEntity.FirstName;
                        user.LastName = appUserEntity.LastName;
                        user.PhoneNumber = appUserEntity.PhoneNumber;
                        user.EmailId = appUserEntity.EmailId;
                        user.NPINumber = appUserEntity.NPINumber;
                        user.Access = appUserEntity.Access;
                         user.createdby = user.createdby;
                        user.createdts = user.createdts;
                        user.updatedby = "Admin";
                        user.updatedts = DateTime.Now;

                        _unitOfWork.AppUserRepo.Update(user);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool DeleteAppUser(int auRowId)
        {
            var success = false;
            if (auRowId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var user = _unitOfWork.AppUserRepo.GetByID(auRowId);
                    if (user != null)
                    {

                        _unitOfWork.AppUserRepo.Delete(user);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }
        public tblAppAdmins GetAdmins(tblAppAdmins admin)
        {
            tblAppAdmins emptyAdmin = new tblAppAdmins();
            if (_ctx.tblAppAdmins.Where(s => s.UserName == admin.UserName && s.Password == admin.Password).Count() > 0)
            {
                return admin;
            }
            return emptyAdmin;
        }
    }
}

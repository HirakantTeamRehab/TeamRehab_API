using System.Linq;
using System.Transactions;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.DataModel;
using Team.Rehab.Repository.UnitOfwork;
using System;

namespace Team.Rehab.Repository.Repository
{
    public class CommonRepository : ICommonRepository
    {
        private readonly IUnitOfwork _unitOfWork;
        private readonly IProcedureManagement _procedureManagement;
        public CommonRepository()
        {
            this._unitOfWork = new UnitOfWork();
            this._procedureManagement = new ProcedureManagement();
        }
        /// <summary>
        /// Get an user based on email address
        /// </summary>
        /// <param name="EmailAddress"></param>
        /// <returns>User object</returns>
        public tblBlockedIP GetIps()
        {

            tblBlockedIP user = _unitOfWork.BlockedIpRepo.Get().ToList().FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            else
            {
                return user;
                // throw new UnauthorizedAccessException();
            }
        }
        public int DeleteUser(int ipID)
        {
            //_unitOfWork.BlockedIpRepo.Delete(lstFailureInfo);
            //return 1;
            // var success = false;
            if (ipID > 0)
            {
                using (var scope = new TransactionScope())
                {
                    // var blockedIp = _unitOfWork.BlockedIpRepo.GetByID(ipID);
                    var blockedIP = _unitOfWork.BlockedIpRepo.Get(o => o.BlockedIPID.Equals(ipID)).FirstOrDefault();
                    if (blockedIP != null)
                    {

                        _unitOfWork.BlockedIpRepo.Delete(blockedIP);
                        _unitOfWork.Save();
                        scope.Complete();
                        // success = true;
                    }
                }
            }
            return 1;

        }
        public int CreateFailureEntry(tblBlockedIP lstFailureInfo)

        {
            _unitOfWork.BlockedIpRepo.Insert(lstFailureInfo);
            _unitOfWork.Save();
            return 1;
        }
        public void UpdateFailureInfo(tblBlockedIP lstFailureInfo)
        {
            // _unitOfWork.BlockedIpRepo.Update(lstFailureInfo);
            tblBlockedIP updatingUser = new tblBlockedIP();
            // var success = false;
            if (lstFailureInfo != null)
            {
                using (var scope = new TransactionScope())
                {

                    if (lstFailureInfo != null)
                    {
                        //product.ProductName = productEntity.ProductName;
                        _unitOfWork.BlockedIpRepo.Update(lstFailureInfo);
                        _unitOfWork.Save();
                        scope.Complete();
                        // success = true;
                    }
                }
            }
            // return success;
        }
        /// <summary>
        /// Fetches IP details by id
        /// </summary>
        /// <param name="ipID"></param>
        /// <returns></returns>
        public tblBlockedIP GetIpsByID(int ipID)
        {
            // var blockedIP = _unitOfWork.BlockedIpRepo.GetByID(ipID);
            var blockedIP = _unitOfWork.BlockedIpRepo.Get(o => o.BlockedIPID.Equals(ipID)).FirstOrDefault();
            //if (blockedIP != null)
            //{
            //    Mapper.CreateMap<Product, ProductEntity>();
            //    var productModel = Mapper.Map<Product, ProductEntity>(product);
            //    return productModel;
            //}
            return blockedIP;
        }

        public void LogAppError(tblAppError AppError)
        {
            try
            {
                _unitOfWork.AppErrorsRepo.Insert(AppError);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
               
                CustomLogger.LogError("CommonRepository in UpdateRefreshToken " + Convert.ToString(ex.Message));
                CustomLogger.SendExcepToDB(ex, "CommonRepository");
                throw;
            }


        }

    }
}

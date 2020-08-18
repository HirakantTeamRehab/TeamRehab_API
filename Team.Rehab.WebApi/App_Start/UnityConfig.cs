using System.Web.Http;
using Resolver;
using Microsoft.Practices.Unity;
using Unity.Mvc3;

using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.Repository.UnitOfwork;
using System.Web.Mvc;
using Team.Rehab.Repository;
using System;
using Team.Rehab.InterfaceRepository.Triarq;
using Team.Rehab.Repository.Repository.Triarq;

namespace Team.Rehab.WebApi.App_Start
{
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        public static void RegisterTypes(IUnityContainer container)
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();        
            //  container.RegisterType<IPatientRepository, PatientRepository>().RegisterType<UnitOfWork>(new HierarchicalLifetimeManager());
            container.RegisterType<IPatientRepository, PatientRepository>();
            container.RegisterType<ITPatientRepository, Team.Rehab.Repository.Repository.Triarq.TPatientRespository>();
            container.RegisterType<INotificationsRepository, NotificationsRepository>();
            container.RegisterType<ICommonRepository, CommonRepository>();
            container.RegisterType<ITNotesRepository, TNotesRepository>();
            container.RegisterType<IAppUsers, AppUsersRepository>();
            container.RegisterType<IDirectTrustPatientRepository, DirectTrustPatientRepository>();
            container.RegisterType<IUnitOfwork, UnitOfWork>();
        }
    }
}
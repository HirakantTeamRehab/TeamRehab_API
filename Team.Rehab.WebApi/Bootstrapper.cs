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

namespace Team.Rehab.WebApi
{
    public static class Bootstrapper
    {

        public static void Initialise()
        {
            var container = BuildUnityContainer();

           System.Web.Mvc.DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            // register dependency resolver for WebAPI RC
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();        
            //  container.RegisterType<IPatientRepository, PatientRepository>().RegisterType<UnitOfWork>(new HierarchicalLifetimeManager());
            container.RegisterType<IPatientRepository, PatientRepository>();
            container.RegisterType<ITPatientRepository, Team.Rehab.Repository.Repository.Triarq.TPatientRespository>();
            container.RegisterType<IUnitOfwork, UnitOfWork>();
            container.RegisterType<ITNotesRepository, TNotesRepository>();
            container.RegisterType<IAppUsers, AppUsersRepository>();
            container.RegisterType<IDirectTrustPatientRepository, DirectTrustPatientRepository>();
           // container.RegisterType<IDirectTrustRefRepository, DirectTrustReferrerRepository>();
            container.RegisterType<IDirectTrustReferrerRepository, DirectTrustReferrerRepository>();
            return container;
        }

    }
}   
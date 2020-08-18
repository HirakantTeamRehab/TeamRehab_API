using System.ComponentModel.Composition;
using Resolver;
using Team.Rehab.InterfaceRepository;
using Team.Rehab.Repository.Repository;
using Team.Rehab.Repository;
using Team.Rehab.Repository.UnitOfwork;

namespace Team.Rehab.Repository
{

    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IPatientRepository, PatientRepository>();
            registerComponent.RegisterType<IDirectTrustPatientRepository, DirectTrustPatientRepository>();
            // registerComponent.RegisterType<IUnitOfwork, UnitOfWork>();
            registerComponent.RegisterType<IDirectTrustReferrerRepository, DirectTrustReferrerRepository>();

        }
    }
}

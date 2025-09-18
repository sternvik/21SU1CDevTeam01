using EntitetsLager;
using DataLager;

namespace AffärsLager.Controllers
{
    public class SystemloggController
    {
        private readonly UnitOfWork _unitOfWork;

        public SystemloggController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // TODO: Implementera systemlogglogik
    }
}
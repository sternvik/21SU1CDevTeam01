using EntitetsLager;
using DataLager;

namespace AffärsLager.Controllers
{
    public class BokningsController
    {
        private readonly UnitOfWork _unitOfWork;

        public BokningsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // TODO: Implementera bokningslogik
    }
}
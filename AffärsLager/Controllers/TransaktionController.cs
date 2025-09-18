using EntitetsLager;
using DataLager;

namespace AffärsLager.Controllers
{
    public class TransaktionController
    {
        private readonly UnitOfWork _unitOfWork;

        public TransaktionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // TODO: Implementera transaktionslogik
    }
}
using EntitetsLager;

namespace DataLager
{
    public interface IUnitOfWork
    {
        IRepository<Medlem> MedlemRepository { get; set; }
        IRepository<MedlemTräningspass> MedlemTräningspassRepository { get; set; }
        IRepository<Tränare> TränareRepository { get; set; }
        IRepository<Träningspass> TräningspassRepository { get; set; }
        IRepository<Utlåning> UtlåningRepository { get; set; }
        IRepository<Utrustning> UtrustningRepository { get; set; }

        void Fill();
        void Save();
    }
}
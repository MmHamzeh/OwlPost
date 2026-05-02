namespace OwlPost.Core.ServicesContract;

public interface IUnitOfWork
{





    
    #region Methods

    Task<int> SaveChanges();
    void RejectChanges();

    #endregion

}
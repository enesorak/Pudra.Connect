using Visus.Cuid;

namespace Pudra.Connect.Domain.Entities;

public abstract class BaseEntity
{
    // set; artık private. Dışarıdan ID değiştirilemez.
    public string Id { get; private set; }

    // BaseEntity'den türeyen bir nesne (new User() gibi) yaratıldığı anda
    // bu constructor çalışır ve ID'yi bir kereliğine atar.
    protected BaseEntity()
    {
        Id = new Cuid2().ToString();

    }
}
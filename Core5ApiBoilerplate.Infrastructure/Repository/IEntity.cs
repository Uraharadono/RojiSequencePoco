namespace Core5ApiBoilerplate.Infrastructure.Repository
{
    public interface IEntity
    {
        long Oid { get; set; }
    }

    public interface IIdentityEntity
    {
        long Id { get; set; }
    }
}

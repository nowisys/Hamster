
namespace Hamster.Plugin.NHibernate
{
    public interface IDataObject
    {
        void SetProperty(string name, object value);
        object GetProperty(string name);
        bool HasProperty(string name);
    }
}

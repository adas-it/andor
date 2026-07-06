namespace Andor.Foundation.Domain.ValuesObjects;

public interface IId<out Tself> where Tself : IId<Tself>
{
    static abstract Tself Empty { get; }
    static abstract Tself New();
    static abstract Tself Load(Guid value);
}

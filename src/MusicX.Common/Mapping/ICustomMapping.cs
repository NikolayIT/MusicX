namespace MusicX.Common.Mapping
{
    using System.Collections.Generic;

    public interface ICustomMapping<TSource, TDestination>
    {
        IEnumerable<CustomMappingPair<TSource, TDestination>> GetCustomMappings();
    }
}

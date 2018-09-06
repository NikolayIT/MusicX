namespace MusicX.Common.Mapping
{
    using System;
    using System.Linq.Expressions;

    public class CustomMappingPair<TSource, TDestination>
    {
        public CustomMappingPair(
            Expression<Func<TSource, object>> sourceMember,
            Expression<Func<TDestination, object>> destinationMember)
        {
            this.SourceMember = sourceMember;
            this.DestinationMember = destinationMember;
        }

        public Expression<Func<TSource, object>> SourceMember { get; set; }

        public Expression<Func<TDestination, object>> DestinationMember { get; set; }
    }
}

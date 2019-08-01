namespace MusicX.Web.Server.Infrastructure.Mapping
{
    using AutoMapper;

    // Simplify and add to the common project
    public interface IHaveCustomMappings
    {
        void CreateMappings(IProfileExpression configuration);
    }
}

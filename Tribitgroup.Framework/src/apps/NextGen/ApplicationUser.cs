using NextGen.Backbone.Backbone.Contracts;

namespace NextGen
{
    public class ApplicationUser : IApplicationUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
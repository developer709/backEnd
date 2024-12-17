using System.Reflection;
using InternalWebsite.Core.Entities;
using Mapster;

namespace InternalWebsite.Core
{
    //!!! Important: Do not Remove this File!
    public class DtosGenerator : ICodeGenerationRegister
    {
        public void Register(CodeGenerationConfig config)
        {
            config.AdaptTwoWays("[name]Dto")
                .ForAllTypesInNamespace(Assembly.GetAssembly(typeof(DtosGenerator)), "InternalWebsite.Core.Entities")
                .ShallowCopyForSameType(true);

            //config.GenerateMapper("[name]Mapper")
            //    .ForAllTypesInNamespace(Assembly.GetAssembly(typeof(DtosGenerator)), "InternalWebsite.Core.Entities")
            //    .ForType<Job>();
        }
    }
}
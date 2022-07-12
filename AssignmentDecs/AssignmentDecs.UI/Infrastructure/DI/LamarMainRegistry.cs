using Lamar;
using Microsoft.Extensions.Configuration;
using AssignmentDecs.UI;
using AssignmentDecs.Data;

namespace AssignmentDecs.UI.Infrastructure.DI
{
    public class LamarMainRegistry : ServiceRegistry
    {
        public LamarMainRegistry(IConfiguration configuration)
        {
            Scan(x =>
            {
                x.Assembly(typeof(Program).Assembly);
                x.WithDefaultConventions();
                x.Assembly("AssignmentDecs.UI");
                x.Assembly("AssignmentDecs.Service");
                x.Assembly("AssignmentDecs.Data");
            });

            var connectionString = configuration.GetConnectionString("AssignmentDecsDBEntities");

            ForConcreteType<AssignmentDecsDBEntities>().Configure
                  .Ctor<string>("AssignmentDecsDBEntities")
                              .Is(connectionString)
                              .Scoped();
        }
    }
}
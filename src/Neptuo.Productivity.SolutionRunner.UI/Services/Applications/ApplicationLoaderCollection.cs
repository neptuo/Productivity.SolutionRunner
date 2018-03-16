using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class ApplicationLoaderCollection : IApplicationLoader
    {
        private readonly List<IApplicationLoader> storage = new List<IApplicationLoader>();

        public ApplicationLoaderCollection Add(IApplicationLoader loader)
        {
            Ensure.NotNull(loader, "loader");
            storage.Add(loader);
            return this;
        }

        public void Add(IApplicationCollection applications)
        {
            foreach (IApplicationLoader loader in storage)
                loader.Add(applications);
        }
    }
}

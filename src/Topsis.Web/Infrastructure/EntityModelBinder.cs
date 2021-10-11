using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Topsis.Adapters.Database;
using Topsis.Domain;

namespace Topsis.Web.Infrastructure
{
    public class EntityModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (typeof(IEntity<string>).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new EntityModelBinder<string>();
            }

            if (typeof(IEntity<int>).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new EntityModelBinder<int>();
            }

            return null;
        }
    }

    public class EntityModelBinder<TKey> : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var original = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (original != ValueProviderResult.None)
            {
                var originalValue = original.FirstValue;
                var dbContext = bindingContext.HttpContext.RequestServices.GetRequiredService<WorkspaceDbContext>();
                IEntity<TKey> entity = null;

                if (typeof(TKey) == typeof(int))
                {
                    if (int.TryParse(originalValue, out var id))
                    {
                        if (bindingContext.ModelType == typeof(JobCategory))
                        {
                            entity = (await dbContext.Set<JobCategory>().FindAsync(id)) as IEntity<TKey>;
                        }
                    }
                }
                else if (bindingContext.ModelType == typeof(Country))
                {
                    entity = (await dbContext.Set<Country>().FindAsync(originalValue)) as IEntity<TKey>;
                }

                bindingContext.Result = entity != null ? ModelBindingResult.Success(entity) : bindingContext.Result;            
            }
        }
    }
}

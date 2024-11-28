namespace Telega;

public class Pipeline<TContext, TResult>(Func<TContext, Task<TResult?>>[] pipes)
{
    public async Task<TResult?> Process(TContext context)
    {
        foreach (var pipe in pipes) {
            var result = await pipe(context);
            if (result != null) return result;
        }

        return default;
    }

    public class Builder
    {
        List<Func<TContext, Task<TResult?>>> pipeline = new();

        public Builder Add(Func<TContext, TResult?> pipe)
        {
            pipeline.Add((context) =>
            {
                var response = pipe(context);
                return Task.FromResult(response)!;
            });

            return this;
        }
        
        public Builder Add(Func<TResult> step)
        {
            pipeline.Add((_) =>
            {
                var response = step();
                return Task.FromResult(response)!;
            });

            return this;
        }

        public Pipeline<TContext, TResult> Build()
        {
            return new(pipeline.ToArray());
        }
    }
}

public static class PipelineServiceCollectionExtensions
{
    public static IServiceCollection AddPipeline<TContext, TResult>(this IServiceCollection services, Action<Pipeline<TContext, TResult>.Builder> configuration)
    {
        var builder = new Pipeline<TContext, TResult>.Builder();

        configuration(builder);

        services.AddSingleton(builder.Build());

        return services;
    }
}

namespace Telega;

public record Unrecognized : IRequest<Response>
{
    public static Unrecognized Instance() => new();
    
    public class Handler : IRequestHandler<Unrecognized, Response>
    {
        public Task<Response> Handle(Unrecognized request, CancellationToken cancellationToken)
        {
            return Task.FromResult<Response>("I don't quite get you");
        }
    }
}
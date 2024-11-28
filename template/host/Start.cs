namespace Telega;

public record Start : IRequest<Response>
{
    public static Start? Recognize(Context context) => context.TextMessage == "/start" ? new Start() : null;

    public class Handler() : IRequestHandler<Start, Response>
    {
        public Task<Response> Handle(Start request, CancellationToken cancellationToken)
        {
            return Task.FromResult<Response>("Hello! Thank you for starting");
        }
    }
}
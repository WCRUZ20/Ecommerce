using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Abstractions
{
    public interface ICommandHandler<TCommand, TResult>
    {
        Task<TResult> HandleAsync(
            TCommand command,
            CancellationToken cancellationToken = default);
    }
}

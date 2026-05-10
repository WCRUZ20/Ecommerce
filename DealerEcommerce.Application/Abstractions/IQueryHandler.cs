using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Abstractions
{
    public interface IQueryHandler<TQuery, TResult>
    {
        Task<TResult> HandleAsync(
            TQuery query,
            CancellationToken cancellationToken = default);
    }
}

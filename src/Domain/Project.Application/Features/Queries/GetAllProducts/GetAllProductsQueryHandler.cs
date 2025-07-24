using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Features.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQueryRequest, GetAllProductsQueryResponse>
    {
        public Task<GetAllProductsQueryResponse> Handle(GetAllProductsQueryRequest request, 
            CancellationToken cancellationToken)
        {
            // Implementation of the query handler logic goes here
            return Task.FromResult(new GetAllProductsQueryResponse());
        }
    }
}

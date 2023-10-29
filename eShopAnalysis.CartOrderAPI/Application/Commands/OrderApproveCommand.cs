using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    //approve aka stock confirmed
    public class OrderApproveCommand: IRequest<IEnumerable<Guid>>
    {
        public IEnumerable<Guid> OrderIdsToApprove { get; private set; }

        public OrderApproveCommand(IEnumerable<Guid> orderIds) {
            OrderIdsToApprove = orderIds ?? throw new ArgumentNullException(nameof(orderIds));
        }


    }
}

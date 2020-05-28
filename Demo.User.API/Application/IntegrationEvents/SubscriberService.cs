using DotNetCore.CAP;
using MediatR;

namespace Demo.User.API.Application.IntegrationEvents
{
    public class SubscriberService : ISubscriberService, ICapSubscribe
    {
        IMediator _mediator;
        public SubscriberService(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[CapSubscribe("PatientCreatedSucceeded")]
        //public void PatientCreatedSucceeded(PatientCreatedDomainEventHandler @event)
        //{
        //    //Do SomeThing
        //}
    }
}

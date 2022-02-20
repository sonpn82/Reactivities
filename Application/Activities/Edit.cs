using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command: IRequest
        {
            public Activity Activity { get; set; }  // request with an Activity object in the body
        }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
      {
        _context = context;
        _mapper = mapper;
      }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        // find the request activity
        var activity = await _context.Activities.FindAsync(request.Activity.Id);

        // update field value by using AutoMapper
        // copy each field val in request.Activity to activity
        _mapper.Map(request.Activity, activity);

        // save
        await _context.SaveChangesAsync();

        return Unit.Value;
      }
    }
  }
}
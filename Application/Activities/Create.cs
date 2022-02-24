using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Create
  {
        // Command do not return any data
      public class Command: IRequest // IRequest is mediator request with a void response
      {
        public Activity Activity { get; set; }  // request with an Activity object in the body
      }
      public class Handler : IRequestHandler<Command>
      {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
          _context = context;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
          // Not use AddAsync here, only add Activity to memory, not to database
          _context.Activities.Add(request.Activity);
          await _context.SaveChangesAsync();

          return Unit.Value; // return just to show the task is completed
        }
      }
  }
}
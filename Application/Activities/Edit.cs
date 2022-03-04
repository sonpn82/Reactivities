using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Edit
  {
    public class Command: IRequest<Result<Unit>>
    {
      public Activity Activity { get; set; }  // request with an Activity object in the body
    }    

    // class for validation of when create an activity
    // also have to add FluentValidation service to the Startup.cs
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(command => command.Activity).SetValidator(new ActivityValidator()); // Validate the activity using our ActivityValidator
      }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
      {
        _context = context;
        _mapper = mapper;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        // find the request activity
        var activity = await _context.Activities.FindAsync(request.Activity.Id);

        if (activity == null) return null;

        // update field value by using AutoMapper
        // copy each field val in request.Activity to activity
        _mapper.Map(request.Activity, activity);

        // save
        var result = await _context.SaveChangesAsync() > 0;

        // error handling for saving
        if (!result) return Result<Unit>.Failure("Failed to update activity");

        return Result<Unit>.Success(Unit.Value);
      }
    }
  }
}